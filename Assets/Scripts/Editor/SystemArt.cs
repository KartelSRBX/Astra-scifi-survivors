using System;
using System.Collections.Generic;

namespace GamingApp.EditorTools
{
    /// <summary>Procedural star systems (menu showcase), per-system nebulae and tileable battle floors.</summary>
    public static partial class UiArt
    {
        public sealed class SystemPalette
        {
            public string name;
            public C star, nebA1, nebA2, nebA3, nebB1, nebB2, nebB3, floor, grid;
            public int planets = 3;
            public bool binary, blackHole;
        }

        public static readonly SystemPalette[] Systems =
        {
            Pal("Sol Outpost",   S(1f, 0.92f, 0.65f), S(0.42f, 0.18f, 0.9f), S(0.14f, 0.26f, 0.95f), S(0.85f, 0.45f, 1f), S(0.08f, 0.62f, 0.72f), S(0.12f, 0.22f, 0.8f), S(0.55f, 0.95f, 1f), S(0.04f, 0.05f, 0.12f), S(0.16f, 0.14f, 0.36f), 3),
            Pal("Kepler Drift",  S(1f, 0.62f, 0.32f), S(0.5f, 0.2f, 0.7f), S(0.8f, 0.35f, 0.5f), S(1f, 0.6f, 0.5f), S(0.2f, 0.2f, 0.6f), S(0.4f, 0.2f, 0.7f), S(0.8f, 0.6f, 1f), S(0.07f, 0.04f, 0.1f), S(0.3f, 0.14f, 0.3f), 2),
            Pal("Vega Reach",    S(0.8f, 0.9f, 1f), S(0.1f, 0.5f, 0.7f), S(0.15f, 0.3f, 0.9f), S(0.6f, 0.9f, 1f), S(0.1f, 0.6f, 0.6f), S(0.1f, 0.3f, 0.7f), S(0.5f, 1f, 0.9f), S(0.03f, 0.07f, 0.1f), S(0.1f, 0.3f, 0.36f), 4),
            Pal("Tau Ceti Veil", S(1f, 0.85f, 0.45f), S(0.1f, 0.55f, 0.4f), S(0.1f, 0.35f, 0.5f), S(0.6f, 1f, 0.7f), S(0.3f, 0.5f, 0.2f), S(0.1f, 0.4f, 0.4f), S(0.8f, 1f, 0.6f), S(0.03f, 0.08f, 0.07f), S(0.1f, 0.32f, 0.26f), 3),
            Pal("Rigel Expanse", S(0.55f, 0.72f, 1f), S(0.12f, 0.2f, 0.8f), S(0.25f, 0.4f, 1f), S(0.7f, 0.8f, 1f), S(0.2f, 0.1f, 0.6f), S(0.1f, 0.3f, 0.9f), S(0.6f, 0.7f, 1f), S(0.03f, 0.04f, 0.13f), S(0.14f, 0.2f, 0.45f), 2),
            Pal("Antares Rift",  S(1f, 0.35f, 0.25f), S(0.6f, 0.1f, 0.25f), S(0.4f, 0.1f, 0.5f), S(1f, 0.4f, 0.4f), S(0.5f, 0.15f, 0.1f), S(0.3f, 0.05f, 0.3f), S(1f, 0.6f, 0.4f), S(0.09f, 0.03f, 0.05f), S(0.36f, 0.1f, 0.18f), 3),
            Pal("Sirius Binary", S(0.9f, 0.95f, 1f), S(0.3f, 0.3f, 0.9f), S(0.55f, 0.35f, 1f), S(0.9f, 0.9f, 1f), S(0.2f, 0.5f, 0.9f), S(0.4f, 0.2f, 0.8f), S(0.8f, 0.9f, 1f), S(0.05f, 0.05f, 0.12f), S(0.22f, 0.2f, 0.46f), 2, binary: true),
            Pal("Polaris Gate",  S(0.5f, 1f, 1f), S(0.1f, 0.7f, 0.7f), S(0.4f, 0.2f, 0.9f), S(0.7f, 1f, 1f), S(0.3f, 0.1f, 0.7f), S(0.1f, 0.6f, 0.8f), S(0.8f, 0.7f, 1f), S(0.03f, 0.07f, 0.1f), S(0.1f, 0.35f, 0.4f), 4),
            Pal("Eta Carinae",   S(1f, 0.5f, 0.8f), S(0.7f, 0.15f, 0.45f), S(0.45f, 0.1f, 0.6f), S(1f, 0.6f, 0.85f), S(0.5f, 0.1f, 0.3f), S(0.3f, 0.1f, 0.5f), S(1f, 0.5f, 0.7f), S(0.08f, 0.03f, 0.08f), S(0.34f, 0.1f, 0.3f), 3),
            Pal("Void Core",     S(1f, 0.55f, 0.3f), S(0.25f, 0.05f, 0.35f), S(0.1f, 0.05f, 0.2f), S(0.8f, 0.3f, 0.6f), S(0.35f, 0.1f, 0.1f), S(0.1f, 0.05f, 0.2f), S(1f, 0.5f, 0.3f), S(0.03f, 0.02f, 0.05f), S(0.25f, 0.1f, 0.28f), 2, blackHole: true),
        };

        static C S(float r, float g, float b) => new C(r, g, b);

        static SystemPalette Pal(string name, C star, C a1, C a2, C a3, C b1, C b2, C b3, C floor, C grid, int planets,
            bool binary = false, bool blackHole = false) => new SystemPalette
        {
            name = name, star = star, nebA1 = a1, nebA2 = a2, nebA3 = a3, nebB1 = b1, nebB2 = b2, nebB3 = b3,
            floor = floor, grid = grid, planets = planets, binary = binary, blackHole = blackHole,
        };

        public static IEnumerable<string> SystemAll()
        {
            for (int i = 1; i <= Systems.Length; i++)
            {
                yield return $"system_{i:00}";
                yield return $"neb_{i:00}_a";
                yield return $"neb_{i:00}_b";
                yield return $"floor_{i:00}";
            }
        }

        static Art MakeSystemArt(string name)
        {
            int i = int.Parse(name.Substring(name.IndexOf('_') + 1, 2)) - 1;
            var s = Systems[i];
            if (name.StartsWith("system_")) return StarSystem(name, s, 100 + i * 17);
            if (name.StartsWith("floor_")) return Floor(name, s, 300 + i * 13);
            if (name.EndsWith("_a")) return Nebula(name, 40 + i * 7, s.nebA1, s.nebA2, s.nebA3);
            if (name.EndsWith("_b")) return Nebula(name, 90 + i * 7, s.nebB1, s.nebB2, s.nebB3);
            throw new ArgumentException("Unknown art: " + name);
        }

        // ---------- star system showcase ----------
        static Art StarSystem(string name, SystemPalette s, int seed)
        {
            const int n = 768;
            var a = new Art(name, n, n);
            var rng = new Random(seed);
            var c = P(n / 2f, n / 2f);
            const float k = 0.34f; // orbit ellipse squash
            float[] orbitR = { 175f, 250f, 320f, 360f };
            var planets = new List<(float r, float ang, float size, C col)>();
            for (int p = 0; p < s.planets; p++)
            {
                var col = C.Lerp(s.nebA3, s.nebB3, (float)rng.NextDouble());
                col = C.Lerp(col, new C(0.5f, 0.55f, 0.7f), 0.35f);
                planets.Add((orbitR[p], (float)(rng.NextDouble() * 360.0), 9f + (float)rng.NextDouble() * 16f, col));
            }

            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    int i = y * n + x;
                    var p = P(x + 0.5f, y + 0.5f);
                    var d = p - c;
                    float r = d.Len;
                    float er = MathF.Sqrt(d.x * d.x + (d.y / k) * (d.y / k));

                    // Soft glow of the whole system.
                    a.Over(i, s.star.WithA(0.45f * MathF.Exp(-r / (s.blackHole ? 150f : 110f))));

                    // Orbits (thin ellipses).
                    for (int o = 0; o < s.planets; o++)
                    {
                        float line = Clamp01(1.2f - MathF.Abs(er - orbitR[o]) * k * 1.4f);
                        if (line > 0f) a.Over(i, C.White.WithA(0.18f * line));
                    }

                    if (s.blackHole)
                    {
                        float disk = AccretionDisk(d, k);
                        if (d.y > 0f) a.Over(i, DiskColor(s, er).WithA(disk * 0.9f));
                        float hole = Clamp01(70f - r + 0.5f);
                        a.Over(i, new C(0.01f, 0f, 0.02f).WithA(hole));
                        float photon = MathF.Exp(-MathF.Pow((r - 73f) / 2.5f, 2f));
                        a.Over(i, new C(1f, 0.85f, 0.7f).WithA(photon));
                        float lens = d.y > 0f ? MathF.Exp(-MathF.Pow((r - 96f) / 5f, 2f)) * Clamp01(d.y / 60f) : 0f;
                        a.Over(i, DiskColor(s, 110f).WithA(lens * 0.9f));
                        if (d.y <= 0f) a.Over(i, DiskColor(s, er).WithA(disk));
                    }
                    else if (s.binary)
                    {
                        Star(a, i, d - P(-70, 18), s.star, 40f);
                        Star(a, i, d - P(70, -18), C.Lerp(s.star, new C(0.6f, 0.8f, 1f), 0.5f), 30f);
                    }
                    else Star(a, i, d, s.star, 58f);

                    foreach (var pl in planets)
                    {
                        float ang = pl.ang * Deg;
                        var pc = P(MathF.Cos(ang) * pl.r, MathF.Sin(ang) * pl.r * k);
                        var q = d - pc;
                        float dist = q.Len;
                        if (dist > pl.size + 8f) continue;
                        a.Over(i, pl.col.WithA(0.35f * MathF.Exp(-MathF.Max(0f, dist - pl.size) / 4f)));
                        if (dist > pl.size + 1f) continue;
                        float nx = q.x / pl.size, ny = q.y / pl.size, nz = MathF.Sqrt(MathF.Max(0f, 1f - nx * nx - ny * ny));
                        var toStar = (P(0, 0) - pc);
                        float tl = MathF.Max(1f, toStar.Len);
                        float lam = MathF.Max(0f, (nx * toStar.x + ny * toStar.y) / tl * 0.85f + nz * 0.35f);
                        var col = pl.col * (0.12f + 1.1f * lam);
                        col = col + s.star * (MathF.Pow(1f - nz, 3f) * 0.4f);
                        a.Over(i, col.WithA(Clamp01(pl.size - dist + 0.5f)));
                    }
                }
            return a;
        }

        static void Star(Art a, int i, V2 d, C col, float radius)
        {
            float r = d.Len;
            float th = MathF.Atan2(d.y, d.x);
            float corona = 0.8f * MathF.Exp(-MathF.Max(0f, r - radius) / (radius * 0.55f));
            float rays = 0.35f * MathF.Pow(MathF.Abs(MathF.Cos(th * 3f)), 24f) * MathF.Exp(-r / (radius * 4f));
            float flare = 0.4f * MathF.Exp(-MathF.Abs(d.y) / 2.2f) * MathF.Exp(-MathF.Abs(d.x) / (radius * 3.5f));
            a.Over(i, col.WithA(Clamp01(corona + rays)));
            a.Over(i, C.White.WithA(flare));
            float body = Clamp01(radius - r + 0.5f);
            if (body > 0f)
            {
                var surf = C.Lerp(C.White, col, Clamp01(r / radius) * 0.7f);
                a.Over(i, surf.WithA(body));
            }
        }

        static float AccretionDisk(V2 d, float k)
        {
            float er = MathF.Sqrt(d.x * d.x + (d.y / k) * (d.y / k));
            float band = Smooth(88f, 110f, er) * (1f - Smooth(190f, 260f, er));
            float streaks = 0.75f + 0.25f * MathF.Sin(er * 0.35f);
            return Clamp01(band * streaks);
        }

        static C DiskColor(SystemPalette s, float er) => C.Lerp(C.Lerp(C.White, s.star, 0.35f), s.nebA3, Clamp01((er - 100f) / 150f));

        // ---------- tileable battle floor ----------
        static float PNoise(float x, float y, int s, int period)
        {
            int xi = (int)MathF.Floor(x), yi = (int)MathF.Floor(y);
            float fx = x - xi, fy = y - yi;
            fx = fx * fx * (3f - 2f * fx); fy = fy * fy * (3f - 2f * fy);
            int x0 = ((xi % period) + period) % period, y0 = ((yi % period) + period) % period;
            int x1 = (x0 + 1) % period, y1 = (y0 + 1) % period;
            return Lerp(Lerp(Hash(x0, y0, s), Hash(x1, y0, s), fx), Lerp(Hash(x0, y1, s), Hash(x1, y1, s), fx), fy);
        }

        static float PFbm(float x, float y, int s, int period, int octaves)
        {
            float sum = 0f, amp = 0.5f, norm = 0f;
            for (int o = 0; o < octaves; o++)
            {
                sum += amp * PNoise(x, y, s + o * 31, period);
                norm += amp;
                x *= 2f; y *= 2f; period *= 2; amp *= 0.5f;
            }
            return sum / norm;
        }

        static Art Floor(string name, SystemPalette s, int seed)
        {
            const int n = 512;
            var a = new Art(name, n, n) { Border = new[] { 0, 0, 0, 0 } };
            var starRng = new Random(seed);
            var stars = new List<(float x, float y, float r, float b)>();
            for (int i = 0; i < 90; i++)
                stars.Add(((float)starRng.NextDouble() * n, (float)starRng.NextDouble() * n, 0.5f + (float)starRng.NextDouble() * 1.1f, 0.3f + 0.5f * (float)starRng.NextDouble()));

            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    int i = y * n + x;
                    float u = (x + 0.5f) / 128f, v = (y + 0.5f) / 128f;
                    float cloud = PFbm(u, v, seed, 4, 4);
                    float cloud2 = PFbm(u + 1.7f, v + 3.1f, seed + 5, 4, 3);
                    var col = s.floor;
                    col = C.Lerp(col, s.nebA1 * 0.55f, Smooth(0.45f, 0.8f, cloud) * 0.45f);
                    col = C.Lerp(col, s.nebB2 * 0.5f, Smooth(0.5f, 0.85f, cloud2) * 0.35f);
                    a.Px[i * 4] = col.r; a.Px[i * 4 + 1] = col.g; a.Px[i * 4 + 2] = col.b; a.Px[i * 4 + 3] = 1f;

                    float gx = Mod(x + 0.5f, 64f), gy = Mod(y + 0.5f, 64f);
                    float lx = MathF.Min(gx, 64f - gx), ly = MathF.Min(gy, 64f - gy);
                    float line = Clamp01(1.3f - MathF.Min(lx, ly));
                    float cross = (lx < 6f && ly < 6f) ? Clamp01(2f - MathF.Min(lx, ly)) : 0f;
                    a.Over(i, s.grid.WithA(0.75f * line));
                    a.Over(i, C.Lerp(s.grid, C.White, 0.4f).WithA(0.8f * cross));

                    foreach (var st in stars)
                    {
                        float dx = MathF.Abs(x + 0.5f - st.x); dx = MathF.Min(dx, n - dx);
                        float dy = MathF.Abs(y + 0.5f - st.y); dy = MathF.Min(dy, n - dy);
                        if (dx > 3f || dy > 3f) continue;
                        float dd = MathF.Sqrt(dx * dx + dy * dy);
                        a.Over(i, C.White.WithA(st.b * Clamp01(st.r + 0.5f - dd)));
                    }
                }
            return a;
        }
    }
}
