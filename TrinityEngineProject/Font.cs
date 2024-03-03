using static StbTrueTypeSharp.StbTrueType;

namespace TrinityEngineProject
{
    internal class Font
    {
        public readonly Texture texture;
        public readonly byte[] bitmap;
        public readonly int bitmapWidth, bitmapHeight;
        public readonly Dictionary<int, GlyphInfo> glyphs = new Dictionary<int, GlyphInfo>();
        public readonly float lineHeight;
        public Font(string path, int width, int height, float fontPixelHeight, float lineHeightMultiplicator = 0.9f, params CharacterRange[] characterRanges)
        {
            lineHeight = fontPixelHeight * lineHeightMultiplicator;

            bitmapWidth = width;
            bitmapHeight = height;
            bitmap = new byte[width * height];
            stbtt_pack_context context = new();
            unsafe
            {
                fixed (byte* pixelsPtr = bitmap)
                {
                    stbtt_PackBegin(context, pixelsPtr, width, height, width, 1, null);
                }
            }

            byte[] ttf = File.ReadAllBytes(path);
            stbtt_fontinfo fontInfo = CreateFont(ttf, 0);
            if (fontInfo == null) TgMessage.ThrowWarning("Failed to init font");

            float scaleFactor = stbtt_ScaleForPixelHeight(fontInfo, fontPixelHeight);

            int ascent, descent, lineGap;
            unsafe
            {
                stbtt_GetFontVMetrics(fontInfo, &ascent, &descent, &lineGap);
            }

            foreach (var range in characterRanges)
            {
                var charData = new stbtt_packedchar[range.Size];
                unsafe
                {
                    fixed (stbtt_packedchar* charDataPtr = charData)
                    {
                        stbtt_PackFontRange(context, fontInfo.data, 0, fontPixelHeight, range.Start, range.Size, charDataPtr);
                    }
                }

                for (int i = 0; i < charData.Length; i++)
                {
                    var yOff = charData[i].yoff + ascent * scaleFactor;

                    var glyphInfo = new GlyphInfo
                    {
                        X0 = (float)charData[i].x0 / width,
                        Y0 = 1f - (float)charData[i].y1 / height,
                        X1 = (float)charData[i].x1 / width,
                        Y1 = 1f - (float)charData[i].y0 / height,
                        Width = charData[i].x1 - charData[i].x0,
                        Height = charData[i].y1 - charData[i].y0,
                        XOffset = (int)charData[i].xoff,
                        YOffset = (int)MathF.Round(charData[i].yoff),
                        XAdvance = (int)MathF.Round(charData[i].xadvance)
                    };
                    Console.WriteLine(glyphInfo.XOffset + " - " + glyphInfo.YOffset +", " + glyphInfo.XAdvance);

                    glyphs.Add(i + range.Start, glyphInfo);
                }
            }
            //stbtt_PackEnd(context);
            byte[] image = new byte[bitmap.Length * 4];
            int iy1 = height - 1;
            for (int iy = 0; iy < height; iy++)
            {
                for (int ix = 0; ix < width; ix++)
                {
                    int i = iy * width + ix;
                    int ind = (iy1 * width + ix) * 4;
                    image[ind] = 255;
                    image[ind + 1] = 255;
                    image[ind + 2] = 255;
                    image[ind + 3] = bitmap[i];
                }
                iy1--;
            }
            texture = new Texture(image, width, height);
        }

        internal struct GlyphInfo
        {
            public float X0, Y0, X1, Y1;
            public int Width, Height;
            public int XOffset, YOffset;
            public int XAdvance;
        }

        internal struct CharacterRange
        {
            //https://jrgraphix.net/r/Unicode/

            public static readonly CharacterRange BasicLatin = new CharacterRange(0x0020, 0x007F);
            public static readonly CharacterRange Latin1Supplement = new CharacterRange(0x00A0, 0x00FF);
            public static readonly CharacterRange LatinExtendedA = new CharacterRange(0x0100, 0x017F);
            public static readonly CharacterRange LatinExtendedB = new CharacterRange(0x0180, 0x024F);
            public static readonly CharacterRange Cyrillic = new CharacterRange(0x0400, 0x04FF);
            public static readonly CharacterRange CyrillicSupplement = new CharacterRange(0x0500, 0x052F);
            public static readonly CharacterRange Hiragana = new CharacterRange(0x3040, 0x309F);
            public static readonly CharacterRange Katakana = new CharacterRange(0x30A0, 0x30FF);
            public static readonly CharacterRange Greek = new CharacterRange(0x0370, 0x03FF);
            public static readonly CharacterRange CjkSymbolsAndPunctuation = new CharacterRange(0x3000, 0x303F);
            public static readonly CharacterRange CjkUnifiedIdeographs = new CharacterRange(0x4e00, 0x9fff);
            public static readonly CharacterRange HangulCompatibilityJamo = new CharacterRange(0x3130, 0x318f);
            public static readonly CharacterRange HangulSyllables = new CharacterRange(0xac00, 0xd7af);

            public int Start { get; }
            public int End { get; }
            public int Size => End - Start + 1;

            public CharacterRange(int start, int end)
            {
                Start = start;
                End = end;
            }
        }
    }
}
