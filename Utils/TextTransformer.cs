using System.Text;
using MegaCrit.Sts2.Core.Localization;

namespace STS2ExclaimEverything.Utils;

internal static class TextTransformer
{
    public static string Transform(string text)
    {
        return Transform(text, ExclaimSettingsService.AppendMissingTerminalExclamation);
    }

    public static string TransformPeriodsOnly(string text)
    {
        return Transform(text, false);
    }

    private static string Transform(string text, bool appendMissingTerminalExclamation)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        Dictionary<int, char>? replacements = null;
        List<int>? insertAfter = null;
        var insideTag = false;
        var insideImageBody = false;
        var tagCloseIndex = -1;
        var lineStartIndex = 0;
        var terminalExclamation = GetCurrentLanguageExclamationMark();

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

            while (insertAfter != null && insertIndex < insertAfter.Count && insertAfter[insertIndex] == i)
            {
                builder.Append(terminalExclamation);
                insertIndex++;
            }
        }

        return builder.ToString();

        void QueueMissingTerminalExclamation(int lineStart, int lineEnd)
        {
            if (!appendMissingTerminalExclamation ||
                !TryGetLineTerminalInfo(text, lineStart, lineEnd, out var lastVisibleIndex,
                    out var terminalChar, out var hasSentenceText) ||
                !hasSentenceText ||
                HasTerminalSymbol(terminalChar))
                return;

            insertAfter ??= [];
            insertAfter.Add(lastVisibleIndex);
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

    private static bool HasTerminalSymbol(char c)
    {
        return char.IsPunctuation(c) || char.IsSymbol(c);
    }

    private static bool TryGetLineTerminalInfo(
        string text,
        int startIndex,
        int endIndex,
        out int lastVisibleIndex,
        out char terminalChar,
        out bool hasSentenceText)
    {
        lastVisibleIndex = -1;
        terminalChar = '\0';
        hasSentenceText = false;

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

            lastVisibleIndex = i;
            terminalChar = GetReplacement(text, i);
            if (char.IsLetter(text[i]))
                hasSentenceText = true;
        }

        return lastVisibleIndex >= 0;
    }

    private static char GetCurrentLanguageExclamationMark()
    {
        try
        {
            return LocManager.Instance?.Language?.ToLowerInvariant() switch
            {
                "zhs" or "jpn" or "kor" => '！',
                _ => '!'
            };
        }
        catch
        {
            return '!';
        }
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