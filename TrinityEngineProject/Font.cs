
using static StbTrueTypeSharp.StbTrueType;

namespace TrinityEngineProject
{
    internal class Font : Material
    {
        public readonly Dictionary<int, GlyphInfo> glyphs;
        public readonly float lineHeight;

        private Font(Texture texture, Shader shader, Dictionary<int, GlyphInfo> glyphs, float lineHeight) : base(shader, texture)
        {
            this.glyphs = glyphs;
            this.lineHeight = lineHeight;
            isFont = true;
        }
        public static Font Create(Shader shader, string path, int bitmapWidth, int bitmapHeight, float fontPixelHeight, float lineHeightMultiplicator = 0.9f, params CharacterRange[] characterRanges)
        {
            LoadFont(path, bitmapWidth, bitmapHeight, fontPixelHeight, characterRanges, 
                out Texture texture, out Dictionary<int, GlyphInfo> glyphs);

            return new Font(texture, shader, glyphs, fontPixelHeight * lineHeightMultiplicator);
        }

        static void LoadFont(string path, int bitmapWidth, int bitmapHeight, float fontPixelHeight, CharacterRange[] characterRanges, out Texture texture, out Dictionary<int, GlyphInfo> glyphs)
        {
            byte[] bitmap = new byte[bitmapWidth * bitmapHeight];
            stbtt_pack_context context = new();
            unsafe
            {
                fixed (byte* pixelsPtr = bitmap)
                {
                    stbtt_PackBegin(context, pixelsPtr, bitmapWidth, bitmapHeight, bitmapWidth, 1, null);
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

            Dictionary<int, GlyphInfo> glyphDict = new Dictionary<int, GlyphInfo>();
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
                        X0 = (float)charData[i].x0 / bitmapWidth,
                        Y0 = 1f - (float)charData[i].y1 / bitmapHeight,
                        X1 = (float)charData[i].x1 / bitmapWidth,
                        Y1 = 1f - (float)charData[i].y0 / bitmapHeight,
                        Width = charData[i].x1 - charData[i].x0,
                        Height = charData[i].y1 - charData[i].y0,
                        XOffset = (int)charData[i].xoff,
                        YOffset = (int)MathF.Round(charData[i].yoff),
                        XAdvance = (int)MathF.Round(charData[i].xadvance)
                    };
                    Console.WriteLine(glyphInfo.XOffset + " - " + glyphInfo.YOffset + ", " + glyphInfo.XAdvance);

                    glyphDict.Add(i + range.Start, glyphInfo);
                }
            }
            //stbtt_PackEnd(context);
            byte[] image = new byte[bitmap.Length * 4];
            int iy1 = bitmapHeight - 1;
            for (int iy = 0; iy < bitmapHeight; iy++)
            {
                for (int ix = 0; ix < bitmapWidth; ix++)
                {
                    int i = iy * bitmapWidth + ix;
                    int ind = (iy1 * bitmapWidth + ix) * 4;
                    image[ind] = 255;
                    image[ind + 1] = 255;
                    image[ind + 2] = 255;
                    image[ind + 3] = bitmap[i];
                }
                iy1--;
            }

            texture = new Texture(image, bitmapWidth, bitmapHeight);
            glyphs = glyphDict;
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
