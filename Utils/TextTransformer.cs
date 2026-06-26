using System.Text;

namespace STS2ExclaimEverything.Utils;

internal static class TextTransformer
{
    public static string Transform(string text)
    {
        return Transform(
            text,
            ExclaimSettingsService.AppendMissingTerminalExclamation,
            ExclaimSettingsService.AppendPureNumericTerminalExclamation);
    }

    public static string TransformPeriodsOnly(string text)
    {
        return Transform(text, false, false);
    }

    private static string Transform(
        string text,
        bool appendMissingTerminalExclamation,
        bool appendPureNumericTerminalExclamation)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        Dictionary<int, char>? replacements = null;
        List<(int Index, char ExclamationMark)>? insertAfter = null;
        var insideTag = false;
        var insideImageBody = false;
        var tagCloseIndex = -1;
        var lineStartIndex = 0;

        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '[' && TryReadBbCodeTag(text, i, out tagCloseIndex, out var isImageOpen,
                    out var isImageClose))
            {
                insideTag = true;
                insideImageBody = insideImageBody && !isImageClose;
                if (isImageOpen)
                    insideImageBody = true;
            }

            if (insideTag || insideImageBody)
            {
                if (insideTag && i == tagCloseIndex)
                    insideTag = false;
                continue;
            }

            if (IsLineBreak(text[i]))
            {
                QueueMissingTerminalExclamation(lineStartIndex, i);
                lineStartIndex = i + 1;
                continue;
            }

            var replacement = GetReplacement(text, i);
            if (replacement == text[i])
                continue;

            replacements ??= [];
            replacements[i] = replacement;
        }

        QueueMissingTerminalExclamation(lineStartIndex, text.Length);

        if (replacements == null && insertAfter == null)
            return text;

        var builder = new StringBuilder(text.Length + (insertAfter?.Count ?? 0));
        var insertIndex = 0;
        for (var i = 0; i < text.Length; i++)
        {
            builder.Append(replacements != null && replacements.TryGetValue(i, out var replacement)
                ? replacement
                : text[i]);

            while (insertAfter != null && insertIndex < insertAfter.Count && insertAfter[insertIndex].Index == i)
            {
                builder.Append(insertAfter[insertIndex].ExclamationMark);
                insertIndex++;
            }
        }

        return builder.ToString();

        void QueueMissingTerminalExclamation(int lineStart, int lineEnd)
        {
            if (!appendMissingTerminalExclamation ||
                !TryGetLineTerminalInfo(text, lineStart, lineEnd, out var insertAfterIndex,
                    out var terminalChar, out var hasAppendableContent, out var hasCjkCharacter,
                    out var isPureNumericContent) ||
                !hasAppendableContent ||
                isPureNumericContent && !appendPureNumericTerminalExclamation ||
                HasBlockingTerminalSymbol(terminalChar))
                return;

            insertAfter ??= [];
            insertAfter.Add((insertAfterIndex, hasCjkCharacter ? '！' : '!'));
        }
    }

    private static char GetReplacement(string text, int index)
    {
        return text[index] switch
        {
            '.' when !IsDecimalPoint(text, index) => '!',
            '。' => '！',
            _ => text[index]
        };
    }

    private static bool IsDecimalPoint(string text, int index)
    {
        return index > 0 &&
               index + 1 < text.Length &&
               char.IsDigit(text[index - 1]) &&
               char.IsDigit(text[index + 1]);
    }

    private static bool IsLineBreak(char c)
    {
        return c is '\r' or '\n';
    }

    private static bool HasBlockingTerminalSymbol(char c)
    {
        if (IsPercentSign(c))
            return false;

        return char.IsPunctuation(c) || char.IsSymbol(c);
    }

    private static bool IsPercentSign(char c)
    {
        return c is '%' or '％';
    }

    private static bool IsOpeningBracket(char c)
    {
        return c is '(' or '（' or '[' or '【' or '{' or '「' or '『' or '《' or '〈';
    }

    private static bool IsClosingBracket(char c)
    {
        return c is ')' or '）' or ']' or '】' or '}' or '」' or '』' or '》' or '〉';
    }

    private static bool IsBracket(char c)
    {
        return IsOpeningBracket(c) || IsClosingBracket(c);
    }

    private static bool IsMatchingBracket(char opening, char closing)
    {
        return (opening, closing) is
            ('(', ')') or
            ('（', '）') or
            ('[', ']') or
            ('【', '】') or
            ('{', '}') or
            ('「', '」') or
            ('『', '』') or
            ('《', '》') or
            ('〈', '〉');
    }

    private static bool IsCjkCharacter(char c)
    {
        return c is >= '\u3400' and <= '\u9FFF' or
            >= '\uF900' and <= '\uFAFF' or
            >= '\u3040' and <= '\u30FF' or
            >= '\uAC00' and <= '\uD7AF';
    }

    private static bool IsNumericSyntax(char c)
    {
        return c is '.' or '．' or ',' or '，' or '%' or '％' or '+' or '-' or '＋' or '－' or '/' or '／';
    }

    private static bool TryGetLineTerminalInfo(
        string text,
        int startIndex,
        int endIndex,
        out int insertAfterIndex,
        out char terminalChar,
        out bool hasAppendableContent,
        out bool hasCjkCharacter,
        out bool isPureNumericContent)
    {
        insertAfterIndex = -1;
        var firstVisibleIndex = -1;
        var lastVisibleIndex = -1;
        var lastNonBracketIndex = -1;
        var hasDigit = false;
        var hasNonNumericContent = false;
        terminalChar = '\0';
        hasAppendableContent = false;
        hasCjkCharacter = false;
        isPureNumericContent = false;

        var insideImageBody = false;
        for (var i = startIndex; i < endIndex; i++)
        {
            if (text[i] == '[' && TryReadBbCodeTag(text, i, out var tagCloseIndex,
                    out var isImageOpen, out var isImageClose))
            {
                insideImageBody = insideImageBody && !isImageClose;
                if (isImageOpen)
                    insideImageBody = true;

                i = tagCloseIndex;
                continue;
            }

            if (insideImageBody)
                continue;

            if (char.IsWhiteSpace(text[i]))
                continue;

            if (firstVisibleIndex < 0)
                firstVisibleIndex = i;

            lastVisibleIndex = i;
            if (!IsBracket(text[i]))
            {
                terminalChar = GetReplacement(text, i);
                lastNonBracketIndex = i;
            }

            if (IsCjkCharacter(text[i]))
                hasCjkCharacter = true;

            if (IsBracket(text[i]))
                continue;

            if (char.IsLetterOrDigit(text[i]))
            {
                hasAppendableContent = true;
                if (char.IsDigit(text[i]))
                    hasDigit = true;
                else
                    hasNonNumericContent = true;
            }
            else if (!IsNumericSyntax(text[i]))
            {
                hasNonNumericContent = true;
            }
        }

        if (lastVisibleIndex < 0 || terminalChar == '\0')
            return false;

        insertAfterIndex = IsFullyWrappedByOuterBrackets(text, firstVisibleIndex, lastVisibleIndex)
            ? lastNonBracketIndex
            : lastVisibleIndex;
        isPureNumericContent = hasDigit && !hasNonNumericContent;
        return insertAfterIndex >= 0;
    }

    private static bool IsFullyWrappedByOuterBrackets(string text, int firstVisibleIndex, int lastVisibleIndex)
    {
        if (firstVisibleIndex < 0 ||
            lastVisibleIndex <= firstVisibleIndex ||
            !IsOpeningBracket(text[firstVisibleIndex]) ||
            !IsMatchingBracket(text[firstVisibleIndex], text[lastVisibleIndex]))
            return false;

        var depth = 0;
        var insideImageBody = false;
        for (var i = firstVisibleIndex; i <= lastVisibleIndex; i++)
        {
            if (text[i] == '[' && TryReadBbCodeTag(text, i, out var tagCloseIndex,
                    out var isImageOpen, out var isImageClose))
            {
                insideImageBody = insideImageBody && !isImageClose;
                if (isImageOpen)
                    insideImageBody = true;

                i = tagCloseIndex;
                continue;
            }

            if (insideImageBody || !IsBracket(text[i]))
                continue;

            if (IsOpeningBracket(text[i]))
                depth++;
            else if (IsClosingBracket(text[i]))
                depth--;

            if (depth == 0 && i < lastVisibleIndex)
                return false;
        }

        return depth == 0;
    }

    private static bool TryReadBbCodeTag(
        string text,
        int index,
        out int closeIndex,
        out bool isImageOpen,
        out bool isImageClose)
    {
        closeIndex = text.IndexOf(']', index + 1);
        isImageOpen = false;
        isImageClose = false;

        if (closeIndex < 0)
            return false;

        var tag = text.AsSpan(index + 1, closeIndex - index - 1).Trim();
        isImageClose = tag.Equals("/img".AsSpan(), StringComparison.OrdinalIgnoreCase);
        isImageOpen = tag.Equals("img".AsSpan(), StringComparison.OrdinalIgnoreCase) ||
                      tag.StartsWith("img=".AsSpan(), StringComparison.OrdinalIgnoreCase);
        return true;
    }
}
