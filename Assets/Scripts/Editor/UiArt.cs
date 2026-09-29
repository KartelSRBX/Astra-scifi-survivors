using System;

namespace GamingApp.EditorTools
{
    public struct V2
    {
        public float x, y;
        public V2(float x, float y) { this.x = x; this.y = y; }
        public static V2 operator +(V2 a, V2 b) => new V2(a.x + b.x, a.y + b.y);
        public static V2 operator -(V2 a, V2 b) => new V2(a.x - b.x, a.y - b.y);
        public static V2 operator *(V2 a, float s) => new V2(a.x * s, a.y * s);
        public float Len => MathF.Sqrt(x * x + y * y);
        public static float Dot(V2 a, V2 b) => a.x * b.x + a.y * b.y;
    }

    public struct C
    {
        public float r, g, b, a;
        public C(float r, float g, float b, float a = 1f) { this.r = r; this.g = g; this.b = b; this.a = a; }
        public static C operator *(C c, float s) => new C(c.r * s, c.g * s, c.b * s, c.a);
        public static C operator +(C x, C y) => new C(x.r + y.r, x.g + y.g, x.b + y.b, x.a);
        public C WithA(float a) => new C(r, g, b, a);
        public static C Lerp(C x, C y, float t) =>
            new C(x.r + (y.r - x.r) * t, x.g + (y.g - x.g) * t, x.b + (y.b - x.b) * t, x.a + (y.a - x.a) * t);
        public static readonly C White = new C(1, 1, 1);
        public static readonly C Clear = new C(0, 0, 0, 0);
    }

    /// <summary>RGBA float image, rows bottom-up (Unity SetPixels order), plus 9-slice border (l, b, r, t).</summary>
    public sealed class Art
    {
        public readonly string Name;
        public readonly int W, H;
        public readonly float[] Px;
        public int[] Border = { 0, 0, 0, 0 };

        public Art(string name, int w, int h) { Name = name; W = w; H = h; Px = new float[w * h * 4]; }

        public void Over(int i, C c)
        {
            if (c.a <= 0f) return;
            int k = i * 4;
            float da = Px[k + 3], a = MathF.Min(1f, c.a), inv = 1f - a;
            float oa = a + da * inv;
            Px[k] = (c.r * a + Px[k] * da * inv) / oa;
            Px[k + 1] = (c.g * a + Px[k + 1] * da * inv) / oa;
            Px[k + 2] = (c.b * a + Px[k + 2] * da * inv) / oa;
            Px[k + 3] = oa;
        }

        /// <summary>Anti-aliased fill of a signed distance field (pixels, negative inside).</summary>
        public void Fill(Func<V2, float> sdf, C c)
        {
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                {
                    float cov = UiArt.Clamp01(0.5f - sdf(new V2(x + 0.5f, y + 0.5f)));
                    if (cov > 0f) Over(y * W + x, c.WithA(c.a * cov));
                }
        }

        public void Paint(Func<V2, C> f)
        {
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                    Over(y * W + x, f(new V2(x + 0.5f, y + 0.5f)));
        }
    }

    /// <summary>Procedural UI art. Engine-independent so it can be rendered and previewed outside Unity.</summary>
    public static class UiArt
    {
        public static readonly string[] All =
        {
            "rr", "rr_line", "rr_soft", "pill", "pill_line", "disc", "ring_thin", "glow", "line_glow", "vignette",
            "grad_bg", "stars_a", "stars_b", "nebula_a", "nebula_b", "hex_field", "ring_tech", "ring_orbit",
            "planet", "orb", "cur_biomass", "cur_cores",
            "ico_play", "ico_character", "ico_inventory", "ico_upgrades", "ico_forge", "ico_gear",
            "ico_left", "ico_right", "ico_lock", "ico_trophy", "ico_clock",
        };

        static readonly C Teal = new C(0.13f, 0.83f, 0.77f);
        static readonly C Violet = new C(0.54f, 0.30f, 1f);
        static readonly C Blue = new C(0.24f, 0.42f, 1f);
        static readonly C Ink = new C(0.03f, 0.04f, 0.10f);
        const float Deg = MathF.PI / 180f;

        public static Art Make(string name)
        {
            switch (name)
            {
                case "rr": return Shape(name, 128, 128, p => Box(p, new V2(64, 64), new V2(62, 62), 36), 48);
                case "rr_line": return Shape(name, 128, 128, p => Ring(Box(p, new V2(64, 64), new V2(60, 60), 34), 4), 48);
                case "rr_soft": return RoundSoft();
                case "pill": return Shape(name, 256, 128, p => Box(p, new V2(128, 64), new V2(126, 62), 62), 63);
                case "pill_line": return Shape(name, 256, 128, p => Ring(Box(p, new V2(128, 64), new V2(124, 60), 60), 4), 63);
                case "disc": return Shape(name, 256, 256, p => Circle(p, new V2(128, 128), 126), 0);
                case "ring_thin": return Shape(name, 256, 256, p => Ring(Circle(p, new V2(128, 128), 122), 5), 0);
                case "glow": return Glow();
                case "line_glow": return LineGlow();
                case "vignette": return Vignette();
                case "grad_bg": return Gradient();
                case "stars_a": return Stars(name, 11);
                case "stars_b": return Stars(name, 29);
                case "nebula_a": return Nebula(name, 5, new C(0.42f, 0.18f, 0.9f), new C(0.14f, 0.26f, 0.95f), new C(0.85f, 0.45f, 1f));
                case "nebula_b": return Nebula(name, 17, new C(0.08f, 0.62f, 0.72f), new C(0.12f, 0.22f, 0.8f), new C(0.55f, 0.95f, 1f));
                case "hex_field": return HexField();
                case "ring_tech": return TechRing();
                case "ring_orbit": return OrbitRing();
                case "planet": return Planet();
                case "orb": return Orb();
                case "cur_biomass": return Biomass();
                case "cur_cores": return Cores();
                case "ico_play": return Icon(name, p => Poly(p, new V2(52, 40), new V2(52, 88), new V2(94, 64)) - 7);
                case "ico_character": return Icon(name, Character);
                case "ico_inventory": return Icon(name, Backpack);
                case "ico_upgrades": return Icon(name, p => Min(
                    Min(Seg(p, new V2(34, 42), new V2(64, 68), 8), Seg(p, new V2(64, 68), new V2(94, 42), 8)),
                    Min(Seg(p, new V2(34, 70), new V2(64, 96), 8), Seg(p, new V2(64, 96), new V2(94, 70), 8))));
                case "ico_forge": return Icon(name, Hammer);
                case "ico_gear": return Icon(name, Gear);
                case "ico_left": return Icon(name, p => Min(Seg(p, new V2(76, 30), new V2(46, 64), 9), Seg(p, new V2(46, 64), new V2(76, 98), 9)));
                case "ico_right": return Icon(name, p => Min(Seg(p, new V2(52, 30), new V2(82, 64), 9), Seg(p, new V2(82, 64), new V2(52, 98), 9)));
                case "ico_lock": return Icon(name, Lock);
                case "ico_trophy": return Icon(name, Trophy);
                case "ico_clock": return Icon(name, p =>
                {
                    var c = new V2(64, 64);
                    return Min(Ring(Circle(p, c, 42), 10), Min(Seg(p, c, c + new V2(0, 26), 5), Seg(p, c, c + new V2(20, -8), 5)));
                });
            }
            throw new ArgumentException("Unknown art: " + name);
        }

        // ---------- math ----------
        public static float Clamp01(float v) => v < 0f ? 0f : v > 1f ? 1f : v;
        static float Lerp(float a, float b, float t) => a + (b - a) * t;
        static float Smooth(float a, float b, float x) { float t = Clamp01((x - a) / (b - a)); return t * t * (3f - 2f * t); }
        static float Min(float a, float b) => MathF.Min(a, b);
        static float Sub(float a, float b) => MathF.Max(a, -b);
        static float Circle(V2 p, V2 c, float r) => (p - c).Len - r;
        static float Ring(float d, float w) => MathF.Abs(d) - w * 0.5f;

        static float Box(V2 p, V2 c, V2 h, float r, float angleDeg = 0f)
        {
            var q = p - c;
            if (angleDeg != 0f)
            {
                float cs = MathF.Cos(angleDeg * Deg), sn = MathF.Sin(angleDeg * Deg);
                q = new V2(q.x * cs + q.y * sn, -q.x * sn + q.y * cs);
            }
            float qx = MathF.Abs(q.x) - h.x + r, qy = MathF.Abs(q.y) - h.y + r;
            return new V2(MathF.Max(qx, 0f), MathF.Max(qy, 0f)).Len + MathF.Min(MathF.Max(qx, qy), 0f) - r;
        }

        static float Seg(V2 p, V2 a, V2 b, float r)
        {
            var pa = p - a; var ba = b - a;
            float h = Clamp01(V2.Dot(pa, ba) / V2.Dot(ba, ba));
            return (pa - ba * h).Len - r;
        }

        static float Poly(V2 p, params V2[] v)
        {
            float d = V2.Dot(p - v[0], p - v[0]), s = 1f;
            for (int i = 0, j = v.Length - 1; i < v.Length; j = i, i++)
            {
                var e = v[j] - v[i]; var w = p - v[i];
                var b = w - e * Clamp01(V2.Dot(w, e) / V2.Dot(e, e));
                d = MathF.Min(d, V2.Dot(b, b));
                bool c1 = p.y >= v[i].y, c2 = p.y < v[j].y, c3 = e.x * w.y > e.y * w.x;
                if ((c1 && c2 && c3) || (!c1 && !c2 && !c3)) s = -s;
            }
            return s * MathF.Sqrt(d);
        }

        static float SMin(float a, float b, float k)
        {
            float h = Clamp01(0.5f + 0.5f * (b - a) / k);
            return Lerp(b, a, h) - k * h * (1f - h);
        }

        static float Hash(int x, int y, int s)
        {
            unchecked
            {
                uint h = (uint)x * 374761393u + (uint)y * 668265263u + (uint)s * 2246822519u;
                h = (h ^ (h >> 13)) * 1274126177u;
                h ^= h >> 16;
                return (h & 0xFFFFFF) / 16777215f;
            }
        }

        static float Noise(float x, float y, int s)
        {
            int xi = (int)MathF.Floor(x), yi = (int)MathF.Floor(y);
            float fx = x - xi, fy = y - yi;
            fx = fx * fx * (3f - 2f * fx); fy = fy * fy * (3f - 2f * fy);
            return Lerp(Lerp(Hash(xi, yi, s), Hash(xi + 1, yi, s), fx), Lerp(Hash(xi, yi + 1, s), Hash(xi + 1, yi + 1, s), fx), fy);
        }

        static float Fbm(float x, float y, int s, int octaves)
        {
            float sum = 0f, amp = 0.5f, norm = 0f;
            for (int i = 0; i < octaves; i++)
            {
                sum += amp * Noise(x, y, s + i * 31);
                norm += amp;
                x = x * 2.03f + 17.1f; y = y * 2.03f + 9.7f; amp *= 0.5f;
            }
            return sum / norm;
        }

        // ---------- frames ----------
        static Art Shape(string name, int w, int h, Func<V2, float> sdf, int border)
        {
            var a = new Art(name, w, h) { Border = new[] { border, border, border, border } };
            if (name.StartsWith("pill")) a.Border = new[] { 64, border, 64, border };
            a.Fill(sdf, C.White);
            return a;
        }

        static Art Icon(string name, Func<V2, float> sdf)
        {
            var a = new Art(name, 128, 128);
            a.Fill(sdf, C.White);
            return a;
        }

        static Art RoundSoft()
        {
            var a = new Art("rr_soft", 192, 192) { Border = new[] { 72, 72, 72, 72 } };
            a.Paint(p =>
            {
                float d = Box(p, new V2(96, 96), new V2(48, 48), 24);
                float al = d <= 0f ? 1f : MathF.Pow(1f - Clamp01(d / 44f), 2.2f);
                return C.White.WithA(al);
            });
            return a;
        }

        static Art Glow()
        {
            var a = new Art("glow", 256, 256);
            a.Paint(p => C.White.WithA(MathF.Pow(Clamp01(1f - (p - new V2(128, 128)).Len / 128f), 2.2f)));
            return a;
        }

        static Art LineGlow()
        {
            var a = new Art("line_glow", 256, 16);
            a.Paint(p =>
            {
                float dy = p.y - 8f;
                float core = MathF.Exp(-dy * dy / (2f * 1.2f * 1.2f)) + 0.35f * MathF.Exp(-dy * dy / (2f * 3.5f * 3.5f));
                return C.White.WithA(Clamp01(core) * Smooth(0f, 90f, p.x) * Smooth(0f, 90f, 256f - p.x));
            });
            return a;
        }

        static Art Vignette()
        {
            var a = new Art("vignette", 256, 256);
            a.Paint(p =>
            {
                float r = ((p - new V2(128, 128)) * (1f / 128f)).Len;
                return new C(0, 0, 0, Smooth(0.5f, 1.4f, r) * 0.95f);
            });
            return a;
        }

        static Art Gradient()
        {
            var a = new Art("grad_bg", 16, 512);
            var bottom = new C(0.015f, 0.02f, 0.06f);
            var mid = new C(0.075f, 0.05f, 0.2f);
            var top = new C(0.025f, 0.045f, 0.13f);
            a.Paint(p =>
            {
                float t = p.y / 512f;
                return t < 0.5f ? C.Lerp(bottom, mid, Smooth(0f, 0.5f, t)) : C.Lerp(mid, top, Smooth(0.5f, 1f, t));
            });
            return a;
        }

        // ---------- space ----------
        static Art Stars(string name, int seed)
        {
            var a = new Art(name, 512, 1024);
            var rng = new Random(seed);
            for (int n = 0; n < 340; n++)
            {
                float sx = (float)rng.NextDouble() * 512f, sy = (float)rng.NextDouble() * 1024f;
                float r = 0.45f + MathF.Pow((float)rng.NextDouble(), 4f) * 2.2f;
                float bright = 0.35f + 0.65f * (float)rng.NextDouble();
                double pick = rng.NextDouble();
                var tint = pick < 0.5 ? C.White : pick < 0.78 ? new C(0.6f, 1f, 0.95f) : new C(0.8f, 0.7f, 1f);
                int ext = (int)MathF.Ceiling(r * 7f) + 2;
                for (int y = (int)sy - ext; y <= (int)sy + ext; y++)
                    for (int x = (int)sx - ext; x <= (int)sx + ext; x++)
                    {
                        if (x < 0 || y < 0 || x >= 512 || y >= 1024) continue;
                        float d = (new V2(x + 0.5f, y + 0.5f) - new V2(sx, sy)).Len;
                        float core = Clamp01(r + 0.5f - d) * MathF.Min(1f, r + 0.3f);
                        float halo = r > 1.2f ? 0.3f * MathF.Exp(-d / (r * 2.2f)) : 0f;
                        a.Over(y * 512 + x, tint.WithA(bright * MathF.Max(core, halo)));
                    }
            }
            return a;
        }

        static Art Nebula(string name, int seed, C ca, C cb, C cc)
        {
            var a = new Art(name, 512, 512);
            a.Paint(p =>
            {
                float u = (p.x - 256f) / 256f, v = (p.y - 256f) / 256f;
                float mask = MathF.Pow(Clamp01(1f - (u * u + v * v)), 1.5f);
                if (mask <= 0f) return C.Clear;
                float n = Fbm(p.x / 110f, p.y / 110f, seed, 5);
                float n2 = Fbm(p.x / 70f + n * 3f, p.y / 70f + n * 3f, seed + 7, 5);
                float dens = Smooth(0.36f, 0.8f, n2) * mask;
                var col = C.Lerp(ca, cb, Smooth(0.3f, 0.7f, Fbm(p.x / 200f, p.y / 200f, seed + 13, 3)));
                col = C.Lerp(col, cc, Smooth(0.62f, 0.85f, n2) * 0.8f);
                return col.WithA(dens * 0.85f);
            });
            return a;
        }

        static Art HexField()
        {
            var a = new Art("hex_field", 1024, 512);
            const float s = 36f;
            float w = MathF.Sqrt(3f) * s, h = 3f * s, apo = s * 0.8660254f;
            a.Paint(p =>
            {
                float ex = (p.x - 512f) / 512f, ey = (p.y - 256f) / 256f;
                float fade = MathF.Pow(Clamp01(1f - (ex * ex + ey * ey)), 1.6f);
                if (fade <= 0f) return C.Clear;
                var ga = new V2(Mod(p.x, w) - w / 2f, Mod(p.y, h) - h / 2f);
                var gb = new V2(Mod(p.x - w / 2f, w) - w / 2f, Mod(p.y - 1.5f * s, h) - h / 2f);
                var g = V2.Dot(ga, ga) < V2.Dot(gb, gb) ? ga : gb;
                float qx = MathF.Abs(g.x), qy = MathF.Abs(g.y);
                float hd = MathF.Max(qx, 0.5f * qx + 0.8660254f * qy);
                float line = Clamp01(1.4f - MathF.Abs(apo - hd));
                float fill = 0.06f;
                return C.White.WithA(MathF.Max(line, fill) * fade);
            });
            return a;
        }

        static float Mod(float x, float m) => x - m * MathF.Floor(x / m);

        static Art TechRing()
        {
            var a = new Art("ring_tech", 512, 512);
            var c = new V2(256, 256);
            a.Paint(p =>
            {
                var d = p - c; float r = d.Len;
                float th = MathF.Atan2(d.y, d.x) / Deg; if (th < 0f) th += 360f;
                float px = r * Deg; // pixels per degree at this radius
                float cov = Clamp01(0.5f - Ring(r - 244f, 3f));

                cov = MathF.Max(cov, Arc(r, th, px, 214f, 8f, 15f, 9f));
                cov = MathF.Max(cov, Arc(r, th, px, 232f, 8f, 6f, 1.1f) * 0.8f);
                float bt = Mod(th - 45f + 14f, 90f); // bold brackets centred on the diagonals
                float bd = MathF.Max(MathF.Abs(r - 196f) - 3f, (bt <= 28f ? -MathF.Min(bt, 28f - bt) : MathF.Min(bt - 28f, 90f - bt)) * px);
                cov = MathF.Max(cov, Clamp01(0.5f - bd));
                return C.White.WithA(cov);
            });
            return a;
        }

        /// <summary>Dashed arc: radius, width, dash period and dash length in degrees.</summary>
        static float Arc(float r, float th, float pxPerDeg, float radius, float width, float period, float dash)
        {
            float local = Mod(th, period);
            float dAng = local <= dash ? -MathF.Min(local, dash - local) : MathF.Min(local - dash, period - local);
            float dd = MathF.Max(MathF.Abs(r - radius) - width * 0.5f, dAng * pxPerDeg);
            return Clamp01(0.5f - dd);
        }

        static Art OrbitRing()
        {
            var a = new Art("ring_orbit", 512, 512);
            var c = new V2(256, 256);
            var sat = c + new V2(MathF.Cos(35f * Deg), MathF.Sin(35f * Deg)) * 238f;
            a.Paint(p =>
            {
                var d = p - c; float r = d.Len;
                float th = MathF.Atan2(d.y, d.x) / Deg; if (th < 0f) th += 360f;
                float trail = Mod(th - 35f, 360f); // brighter just behind the satellite
                float ring = Clamp01(0.5f - Ring(r - 238f, 2.5f)) * (0.35f + 0.65f * MathF.Pow(Clamp01(trail / 360f), 3f));
                float ds = (p - sat).Len;
                float dot = MathF.Max(Clamp01(8.5f - ds), 0.7f * MathF.Exp(-ds / 10f));
                return C.White.WithA(MathF.Max(ring, dot));
            });
            return a;
        }

        static Art Planet()
        {
            const int n = 768;
            var a = new Art("planet", n, n);
            const float R = 210f, ri = 262f, ro = 362f, k = 0.24f;
            float ct = MathF.Cos(-16f * Deg), st = MathF.Sin(-16f * Deg);
            float lx = -0.55f, ly = 0.55f, lz = 0.63f;
            float ll = MathF.Sqrt(lx * lx + ly * ly + lz * lz); lx /= ll; ly /= ll; lz /= ll;
            var deep = new C(0.2f, 0.08f, 0.42f);
            var halo = new C(0.35f, 0.62f, 1f);

            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    int i = y * n + x;
                    float px = x + 0.5f - n / 2f, py = y + 0.5f - n / 2f;
                    float dist = MathF.Sqrt(px * px + py * py);
                    float u = px * ct + py * st, v = -px * st + py * ct;

                    if (dist > R - 2f) a.Over(i, halo.WithA(0.55f * MathF.Exp(-MathF.Max(0f, dist - R) / 20f)));

                    float er = MathF.Sqrt(u * u + (v / k) * (v / k));
                    C ring = C.Clear;
                    if (er > ri - 1f && er < ro + 1f)
                    {
                        float cov = Clamp01(er - ri + 0.5f) * Clamp01(ro - er + 0.5f);
                        float bands = 0.55f + 0.45f * MathF.Sin(er * 0.23f) * MathF.Sin(er * 0.057f + 1f);
                        float gap = 1f - 0.85f * (Smooth(312f, 316f, er) * (1f - Smooth(322f, 326f, er)));
                        float t = (er - ri) / (ro - ri);
                        var col = C.Lerp(new C(0.35f, 0.92f, 0.95f), new C(0.68f, 0.48f, 1f), t) * (0.7f + 0.3f * bands);
                        ring = col.WithA(0.8f * bands * cov * gap * (v < 0f ? 1f : 0.75f));
                    }
                    if (v >= 0f) a.Over(i, ring);

                    if (dist < R + 1f)
                    {
                        float nx = px / R, ny = py / R, nz = MathF.Sqrt(MathF.Max(0f, 1f - nx * nx - ny * ny));
                        float lam = MathF.Max(0f, nx * lx + ny * ly + nz * lz);
                        float n1 = Fbm(u / 90f + 5f, v / 14f + 3f, 11, 4);
                        float band = 0.5f + 0.5f * MathF.Sin(v / R * 11f + n1 * 5f);
                        var col = C.Lerp(deep, new C(0.2f, 0.34f, 0.88f), band);
                        float n2 = Fbm(u / 80f, v / 22f, 23, 4);
                        col = C.Lerp(col, new C(0.16f, 0.68f, 0.78f), Smooth(0.58f, 0.78f, n2) * 0.6f);
                        float su = (u + 55f) / 42f, sv = (v - 48f) / 17f;
                        float storm = 1f - Smooth(0.5f, 1f, su * su + sv * sv);
                        col = C.Lerp(col, new C(0.78f, 0.5f, 1f), storm * 0.75f);
                        col = col * (0.1f + 1.05f * MathF.Pow(lam, 0.9f));
                        float fres = MathF.Pow(1f - nz, 2.5f);
                        float back = Clamp01(-(nx * lx + ny * ly) * 1.3f + 0.25f);
                        col = col + Teal * (fres * 0.9f * back) + new C(0.3f, 0.45f, 1f) * (fres * 0.25f);
                        col = col + C.White * (MathF.Pow(lam, 14f) * 0.22f);
                        a.Over(i, col.WithA(Clamp01(R - dist + 0.5f)));
                    }

                    if (v < 0f) a.Over(i, ring);
                }
            return a;
        }

        static Art Orb()
        {
            var a = new Art("orb", 256, 256);
            var c = new V2(128, 128);
            const float R = 122f;
            a.Paint(p =>
            {
                float dist = (p - c).Len;
                if (dist > R + 1f) return C.Clear;
                float t = Clamp01((p - new V2(100, 162)).Len / (R * 1.55f));
                var col = C.Lerp(new C(0.74f, 0.52f, 1f), new C(0.22f, 0.2f, 0.78f), t);
                float edge = Clamp01(1f - (R - dist) / 24f);
                col = C.Lerp(col, Teal, edge * edge * 0.75f);
                float inner = Clamp01(1f - MathF.Abs(R - 16f - dist) / 2.5f) * 0.35f;
                col = C.Lerp(col, C.White, inner);
                var q = p - new V2(94, 180);
                float cs = MathF.Cos(-30f * Deg), sn = MathF.Sin(-30f * Deg);
                float hx = (q.x * cs + q.y * sn) / 48f, hy = (-q.x * sn + q.y * cs) / 24f;
                col = C.Lerp(col, C.White, (1f - Smooth(0.2f, 1f, hx * hx + hy * hy)) * 0.4f);
                return col.WithA(Clamp01(R - dist + 0.5f));
            });
            return a;
        }

        // ---------- currencies (baked colour) ----------
        static float Droplet(V2 p) => SMin(Circle(p, new V2(64, 52), 36), Poly(p, new V2(64, 122), new V2(36, 60), new V2(92, 60)), 8f);

        static Art Biomass()
        {
            var a = new Art("cur_biomass", 128, 128);
            a.Fill(p => Droplet(p) - 5f, Ink);
            a.Paint(p =>
            {
                float d = Droplet(p);
                if (d > 1f) return C.Clear;
                float t = Clamp01((p.y - 14f) / 104f);
                var col = C.Lerp(new C(0.02f, 0.4f, 0.46f), new C(0.4f, 1f, 0.9f), t);
                float cells = MathF.Min(Circle(p, new V2(50, 46), 10), MathF.Min(Circle(p, new V2(78, 40), 7), Circle(p, new V2(72, 66), 6)));
                col = C.Lerp(col, new C(0.75f, 1f, 0.95f), Clamp01(0.5f - cells) * 0.45f);
                col = C.Lerp(col, new C(0.02f, 0.3f, 0.35f), Clamp01(1f - MathF.Abs(d + 4f) / 2f) * 0.5f);
                var q = p - new V2(48, 72);
                col = C.Lerp(col, C.White, (1f - Smooth(0.4f, 1f, (q.x / 9f) * (q.x / 9f) + (q.y / 16f) * (q.y / 16f))) * 0.7f);
                return col.WithA(Clamp01(0.5f - d));
            });
            return a;
        }

        static Art Cores()
        {
            var a = new Art("cur_cores", 128, 128);
            var c = new V2(64, 64);
            var v = new V2[6];
            for (int i = 0; i < 6; i++) v[i] = c + new V2(MathF.Cos((90f + 60f * i) * Deg), MathF.Sin((90f + 60f * i) * Deg)) * 54f;
            a.Fill(p => Poly(p, v) - 5f, Ink);
            float[] shade = { 1.25f, 0.95f, 0.7f, 0.55f, 0.75f, 1.05f };
            var baseCol = new C(0.58f, 0.36f, 1f);
            for (int i = 0; i < 6; i++)
            {
                var v0 = v[i]; var v1 = v[(i + 1) % 6]; float sh = shade[i];
                a.Fill(p => Poly(p, c, v0, v1) - 0.6f, baseCol * sh);
            }
            var inner = new V2[6];
            for (int i = 0; i < 6; i++) inner[i] = c + (v[i] - c) * 0.4f;
            a.Fill(p => Poly(p, inner), new C(0.9f, 0.82f, 1f, 0.95f));
            a.Fill(p => Circle(p, new V2(52, 94), 5f), C.White.WithA(0.8f));
            return a;
        }

        // ---------- icons (white, tinted in UI) ----------
        static float Character(V2 p)
        {
            float head = Sub(Circle(p, new V2(64, 82), 28), Box(p, new V2(64, 84), new V2(18, 8), 7));
            float body = MathF.Max(Box(p, new V2(64, 24), new V2(44, 22), 20), 10f - p.y);
            return Min(head, body);
        }

        static float Backpack(V2 p)
        {
            float body = Box(p, new V2(64, 52), new V2(36, 40), 14);
            float handle = MathF.Max(Ring(Circle(p, new V2(64, 92), 16), 8), 90f - p.y);
            float shape = Min(body, handle);
            shape = Sub(shape, Ring(Box(p, new V2(64, 36), new V2(20, 13), 6), 5));
            shape = Sub(shape, Seg(p, new V2(30, 68), new V2(98, 68), 3));
            return shape;
        }

        static float Hammer(V2 p)
        {
            float handle = Seg(p, new V2(30, 30), new V2(76, 76), 7);
            float head = Box(p, new V2(80, 80), new V2(32, 14), 5, 135f);
            float spark = Min(Circle(p, new V2(30, 96), 5), Min(Circle(p, new V2(22, 78), 3.5f), Circle(p, new V2(44, 108), 3.5f)));
            return Min(Min(handle, head), spark);
        }

        static float Gear(V2 p)
        {
            var c = new V2(64, 64);
            float g = Circle(p, c, 32);
            for (int i = 0; i < 8; i++)
            {
                float ang = i * 45f;
                var tc = c + new V2(MathF.Cos(ang * Deg), MathF.Sin(ang * Deg)) * 40f;
                g = Min(g, Box(p, tc, new V2(10, 9), 3, ang));
            }
            return Sub(g, Circle(p, c, 13));
        }

        static float Lock(V2 p)
        {
            float body = Box(p, new V2(64, 48), new V2(34, 28), 8);
            float shackle = MathF.Max(Ring(Circle(p, new V2(64, 78), 22), 10), 70f - p.y);
            float hole = Min(Circle(p, new V2(64, 54), 7), Box(p, new V2(64, 42), new V2(3.5f, 9), 2));
            return Sub(Min(body, shackle), hole);
        }

        static float Trophy(V2 p)
        {
            float bowl = MathF.Max(Circle(p, new V2(64, 78), 28), p.y - 98f);
            float rim = Box(p, new V2(64, 99), new V2(32, 5), 2.5f);
            float hl = MathF.Max(Ring(Circle(p, new V2(36, 84), 12), 6), p.x - 38f);
            float hr = MathF.Max(Ring(Circle(p, new V2(92, 84), 12), 6), 90f - p.x);
            float stem = Box(p, new V2(64, 42), new V2(5, 12), 1);
            float plate = Box(p, new V2(64, 26), new V2(24, 7), 3);
            return Min(Min(Min(bowl, rim), Min(hl, hr)), Min(stem, plate));
        }
    }
}
