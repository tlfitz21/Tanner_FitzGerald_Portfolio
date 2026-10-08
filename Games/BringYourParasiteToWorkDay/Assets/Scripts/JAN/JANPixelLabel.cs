using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Chunks a short string into a point-filtered texture so the status bar reads like DOOM's font.
public class JANPixelLabel : MonoBehaviour
{
    static readonly Dictionary<char, byte[]> Glyphs = BuildGlyphs();

    RawImage image;
    Texture2D texture;
    string shown = "";
    Color shownColor;
    int scale = 4;

    public void Set(string text, Color color, int pixelScale)
    {
        if (text == null)
            text = "";
        if (text == shown && color == shownColor && pixelScale == scale && texture != null)
            return;

        shown = text;
        shownColor = color;
        scale = Mathf.Max(1, pixelScale);
        Rebuild();
    }

    void Rebuild()
    {
        const int gw = 5;
        const int gh = 7;
        int length = Mathf.Max(1, shown.Length);
        int width = length * (gw + 1) - 1;
        int height = gh;
        if (texture == null || texture.width != width || texture.height != height)
        {
            if (texture != null)
                Destroy(texture);
            texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
        }

        Color[] pixels = new Color[width * height];
        Color clear = new Color(0f, 0f, 0f, 0f);
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = clear;

        for (int c = 0; c < shown.Length; c++)
        {
            if (!Glyphs.TryGetValue(char.ToUpperInvariant(shown[c]), out byte[] rows))
                continue;
            int ox = c * (gw + 1);
            for (int y = 0; y < gh; y++)
            {
                byte row = rows[y];
                for (int x = 0; x < gw; x++)
                {
                    if ((row & (1 << (gw - 1 - x))) == 0)
                        continue;
                    int py = gh - 1 - y;
                    pixels[py * width + ox + x] = shownColor;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply(false, false);
        if (image == null)
            image = gameObject.GetComponent<RawImage>();
        if (image == null)
            image = gameObject.AddComponent<RawImage>();
        image.texture = texture;
        image.color = Color.white;
        image.raycastTarget = false;
        RectTransform rect = (RectTransform)transform;
        rect.sizeDelta = new Vector2(width * scale, height * scale);
    }

    void OnDestroy()
    {
        if (texture != null)
            Destroy(texture);
    }

    static Dictionary<char, byte[]> BuildGlyphs()
    {
        Dictionary<char, byte[]> map = new Dictionary<char, byte[]>();
        Add(map, ' ', "00000", "00000", "00000", "00000", "00000", "00000", "00000");
        Add(map, '0', "01110", "10001", "10011", "10101", "11001", "10001", "01110");
        Add(map, '1', "00100", "01100", "00100", "00100", "00100", "00100", "01110");
        Add(map, '2', "01110", "10001", "00001", "00010", "00100", "01000", "11111");
        Add(map, '3', "11110", "00001", "00001", "01110", "00001", "00001", "11110");
        Add(map, '4', "00010", "00110", "01010", "10010", "11111", "00010", "00010");
        Add(map, '5', "11111", "10000", "11110", "00001", "00001", "10001", "01110");
        Add(map, '6', "00110", "01000", "10000", "11110", "10001", "10001", "01110");
        Add(map, '7', "11111", "00001", "00010", "00100", "01000", "01000", "01000");
        Add(map, '8', "01110", "10001", "10001", "01110", "10001", "10001", "01110");
        Add(map, '9', "01110", "10001", "10001", "01111", "00001", "00010", "01100");
        Add(map, '%', "11001", "11010", "00010", "00100", "01000", "01011", "10011");
        Add(map, ':', "00000", "00100", "00100", "00000", "00100", "00100", "00000");
        Add(map, '-', "00000", "00000", "00000", "11111", "00000", "00000", "00000");
        Add(map, 'A', "01110", "10001", "10001", "11111", "10001", "10001", "10001");
        Add(map, 'B', "11110", "10001", "10001", "11110", "10001", "10001", "11110");
        Add(map, 'C', "01110", "10001", "10000", "10000", "10000", "10001", "01110");
        Add(map, 'D', "11110", "10001", "10001", "10001", "10001", "10001", "11110");
        Add(map, 'E', "11111", "10000", "10000", "11110", "10000", "10000", "11111");
        Add(map, 'F', "11111", "10000", "10000", "11110", "10000", "10000", "10000");
        Add(map, 'G', "01110", "10001", "10000", "10111", "10001", "10001", "01110");
        Add(map, 'H', "10001", "10001", "10001", "11111", "10001", "10001", "10001");
        Add(map, 'I', "01110", "00100", "00100", "00100", "00100", "00100", "01110");
        Add(map, 'J', "00111", "00010", "00010", "00010", "10010", "10010", "01100");
        Add(map, 'K', "10001", "10010", "10100", "11000", "10100", "10010", "10001");
        Add(map, 'L', "10000", "10000", "10000", "10000", "10000", "10000", "11111");
        Add(map, 'M', "10001", "11011", "10101", "10101", "10001", "10001", "10001");
        Add(map, 'N', "10001", "11001", "10101", "10011", "10001", "10001", "10001");
        Add(map, 'O', "01110", "10001", "10001", "10001", "10001", "10001", "01110");
        Add(map, 'P', "11110", "10001", "10001", "11110", "10000", "10000", "10000");
        Add(map, 'Q', "01110", "10001", "10001", "10001", "10101", "10010", "01101");
        Add(map, 'R', "11110", "10001", "10001", "11110", "10100", "10010", "10001");
        Add(map, 'S', "01111", "10000", "10000", "01110", "00001", "00001", "11110");
        Add(map, 'T', "11111", "00100", "00100", "00100", "00100", "00100", "00100");
        Add(map, 'U', "10001", "10001", "10001", "10001", "10001", "10001", "01110");
        Add(map, 'V', "10001", "10001", "10001", "10001", "10001", "01010", "00100");
        Add(map, 'W', "10001", "10001", "10001", "10101", "10101", "10101", "01010");
        Add(map, 'X', "10001", "10001", "01010", "00100", "01010", "10001", "10001");
        Add(map, 'Y', "10001", "10001", "01010", "00100", "00100", "00100", "00100");
        Add(map, 'Z', "11111", "00001", "00010", "00100", "01000", "10000", "11111");
        return map;
    }

    static void Add(Dictionary<char, byte[]> map, char glyph, params string[] rows)
    {
        byte[] bits = new byte[rows.Length];
        for (int y = 0; y < rows.Length; y++)
        {
            byte value = 0;
            string row = rows[y];
            for (int x = 0; x < row.Length && x < 5; x++)
            {
                if (row[x] != '0')
                    value |= (byte)(1 << (4 - x));
            }
            bits[y] = value;
        }
        map[glyph] = bits;
    }
}
