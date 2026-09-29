using System;

namespace GamingApp.EditorTools
{
    /// <summary>Procedural in-game art: enemy craft (black/red, darker = stronger), player ships, projectiles, item icons.</summary>
    public static partial class UiArt
    {
        public static readonly string[] GameAll =
        {
            "enemy_01", "enemy_02", "enemy_03", "enemy_04", "enemy_05",
            "enemy_06", "enemy_07", "enemy_08", "enemy_09", "enemy_10",
            "ship_corvette", "ship_frigate", "ship_destroyer", "ship_cruiser", "ship_dreadnought",
            "bolt_player", "bolt_enemy", "gem_xp",
            "ico_weapon", "ico_hull", "ico_engine", "ico_module", "ico_ship",
        };

        struct Style
        {
            public C hull, dark, accent, glow, cockpit;
            public float aura, rim;
        }

        static readonly C EnemyRed = new C(1f, 0.1f, 0.15f);

        /// <summary>Tier 1..10: hull goes from blood red to near black; the aura grows with tier.</summary>
        static Style EnemyStyle(int tier)
        {
            float f = MathF.Pow((tier - 1) / 9f, 0.8f);
            var hull = C.Lerp(new C(0.62f, 0.1f, 0.12f), new C(0.11f, 0.09f, 0.11f), f);
            return new Style
            {
                hull = hull, dark = hull * 0.28f, accent = EnemyRed, glow = new C(1f, 0.12f, 0.15f),
                cockpit = EnemyRed, aura = 0.08f + 0.3f * f, rim = 0.85f,
            };
        }

        static readonly Style PlayerStyle = new Style
        {
            hull = new C(0.84f, 0.89f, 0.98f), dark = new C(0.26f, 0.3f, 0.45f), accent = Teal,
            glow = new C(0.25f, 0.95f, 0.9f), cockpit = new C(0.6f, 0.4f, 1f), aura = 0.1f, rim = 0.45f,
        };

        static V2 P(float x, float y) => new V2(x, y);
        /// <summary>Folds the point onto the right half so shapes can be authored once and mirrored.</summary>
        static V2 M(V2 p) => new V2(64f + MathF.Abs(p.x - 64f), p.y);

        static Art MakeGame(string name)
        {
            switch (name)
            {
                case "enemy_01": return Craft(name, 256, EnemyStyle(1),
                    p => Poly(p, P(64, 112), P(88, 30), P(64, 44), P(40, 30)) - 2f,
                    p => Seg(p, P(64, 98), P(64, 56), 0f),
                    p => Circle(p, P(64, 72), 5f), null, new[] { P(64, 44) }, 6f);
                case "enemy_02": return Craft(name, 256, EnemyStyle(2),
                    p => Min(Box(p, P(64, 62), P(9, 40), 8), Poly(M(p), P(70, 84), P(108, 56), P(102, 38), P(70, 54)) - 1f),
                    p => Seg(M(p), P(72, 70), P(100, 50), 0f),
                    p => Min(Circle(M(p), P(69, 94), 3.5f), Seg(p, P(64, 104), P(64, 110), 2f)), null, new[] { P(64, 22) }, 7f);
                case "enemy_03": return Craft(name, 256, EnemyStyle(3),
                    p => SMin(Circle(p, P(64, 58), 30), Seg(M(p), P(76, 80), P(86, 112), 5), 6f),
                    p => Circle(p, P(64, 58), 19),
                    p => Min(Circle(p, P(64, 58), 9), Circle(M(p), P(86, 112), 3)), null, new[] { P(52, 30), P(76, 30) }, 6f);
                case "enemy_04": return Craft(name, 256, EnemyStyle(4),
                    p => Min(Min(Box(p, P(64, 56), P(24, 34), 20), Box(p, P(64, 98), P(7, 18), 3)), Poly(M(p), P(84, 60), P(104, 34), P(84, 40))),
                    p => Box(p, P(64, 56), P(14, 22), 10),
                    p => Min(Circle(p, P(64, 114), 5), Box(M(p), P(74, 72), P(4, 2), 1)), null, new[] { P(54, 24), P(74, 24) }, 7f);
                case "enemy_05": return Craft(name, 256, EnemyStyle(5),
                    p => Poly(p, P(64, 114), P(114, 42), P(98, 26), P(64, 60), P(30, 26), P(14, 42)) - 1f,
                    p => Seg(M(p), P(64, 98), P(100, 44), 0f),
                    p => Min(Circle(p, P(64, 82), 6), Circle(M(p), P(104, 40), 3)), null, new[] { P(42, 34), P(86, 34) }, 7f);
                case "enemy_06": return Craft(name, 256, EnemyStyle(6),
                    p => Min(Min(Poly(p, Hex(P(64, 62), 46)) - 3f, Box(p, P(64, 104), P(20, 8), 3)), Box(M(p), P(108, 62), P(6, 24), 3)),
                    p => Poly(p, Hex(P(64, 62), 30)),
                    p => Min(Box(p, P(64, 72), P(16, 2.5f), 1.5f), Min(Box(p, P(64, 62), P(12, 2.5f), 1.5f), Box(p, P(64, 52), P(8, 2.5f), 1.5f))),
                    null, new[] { P(46, 18), P(64, 16), P(82, 18) }, 7f);
                case "enemy_07": return Craft(name, 256, EnemyStyle(7),
                    p => Min(Poly(p, P(64, 124), P(76, 64), P(78, 22), P(50, 22), P(52, 64)), Seg(M(p), P(74, 52), P(98, 20), 5)),
                    p => Seg(p, P(64, 100), P(64, 30), 0f),
                    p => Min(Seg(p, P(64, 116), P(64, 76), 1.8f), Circle(M(p), P(98, 20), 3.5f)), null, new[] { P(64, 20) }, 8f);
                case "enemy_08": return Craft(name, 256, EnemyStyle(8),
                    p => Min(Sub(Circle(p, P(64, 58), 50), Circle(p, P(64, 20), 46)), Box(p, P(64, 62), P(12, 36), 10)),
                    p => Circle(p, P(64, 58), 40),
                    p => Min(Circle(p, P(64, 76), 8), Circle(M(p), P(110, 40), 4)), null, new[] { P(64, 28) }, 8f);
                case "enemy_09": return Craft(name, 512, EnemyStyle(9), Warden,
                    p => Circle(p, P(64, 64), 18), WardenLights, null, null, 0f);
                case "enemy_10": return Craft(name, 512, EnemyStyle(10), Leviathan,
                    p => Min(Seg(M(p), P(70, 90), P(110, 50), 0f), Seg(p, P(64, 110), P(64, 30), 0f)),
                    p => Min(Min(Circle(M(p), P(72, 96), 5), Circle(p, P(64, 62), 10)), Seg(M(p), P(78, 70), P(104, 48), 1.5f)),
                    null, new[] { P(44, 22), P(64, 18), P(84, 22) }, 8f);

                case "ship_corvette": return Craft(name, 256, PlayerStyle,
                    p => Poly(p, P(64, 120), P(76, 66), P(96, 32), P(78, 36), P(64, 26), P(50, 36), P(32, 32), P(52, 66)),
                    p => Seg(M(p), P(64, 90), P(80, 42), 0f),
                    p => Min(Seg(p, P(64, 64), P(64, 42), 1.5f), Circle(M(p), P(92, 34), 2.5f)),
                    p => Box(p, P(64, 84), P(5, 11), 5), new[] { P(64, 28) }, 7f);
                case "ship_frigate": return Craft(name, 256, PlayerStyle,
                    p => Min(Box(p, P(64, 66), P(13, 48), 12), Min(Poly(M(p), P(74, 74), P(106, 42), P(106, 30), P(74, 42)), Box(M(p), P(104, 44), P(6, 18), 5))),
                    p => Seg(p, P(64, 104), P(64, 30), 0f),
                    p => Min(Circle(M(p), P(104, 62), 3), Seg(M(p), P(78, 58), P(100, 40), 1.4f)),
                    p => Box(p, P(64, 94), P(6, 10), 6), new[] { P(56, 18), P(72, 18) }, 7f);
                case "ship_destroyer": return Craft(name, 256, PlayerStyle,
                    p => Min(Poly(p, P(64, 124), P(80, 92), P(82, 30), P(72, 14), P(56, 14), P(46, 30), P(48, 92)), Poly(M(p), P(82, 68), P(118, 36), P(114, 24), P(82, 42))),
                    p => Min(Seg(p, P(64, 88), P(64, 24), 0f), Box(p, P(64, 56), P(12, 20), 4)),
                    p => Min(Seg(M(p), P(86, 56), P(110, 34), 1.6f), Circle(M(p), P(72, 76), 3)),
                    p => Box(p, P(64, 102), P(6, 12), 6), new[] { P(56, 16), P(72, 16) }, 8f);
                case "ship_cruiser": return Craft(name, 256, PlayerStyle,
                    p => Min(Box(p, P(64, 62), P(22, 52), 16), Poly(M(p), P(84, 84), P(124, 42), P(120, 20), P(84, 34))),
                    p => Min(Box(p, P(64, 58), P(12, 34), 8), Seg(M(p), P(86, 60), P(118, 32), 0f)),
                    p => Min(Circle(M(p), P(80, 86), 5), Circle(M(p), P(116, 26), 3)),
                    p => Box(p, P(64, 100), P(8, 9), 6), new[] { P(50, 12), P(64, 10), P(78, 12) }, 8f);
                case "ship_dreadnought": return Craft(name, 512, PlayerStyle,
                    p => Min(Poly(p, P(64, 126), P(90, 100), P(96, 30), P(84, 8), P(44, 8), P(32, 30), P(38, 100)),
                        Min(Box(M(p), P(112, 56), P(11, 38), 8), Box(M(p), P(98, 56), P(8, 10), 3))),
                    p => Min(Seg(p, P(64, 98), P(64, 20), 0f), Min(Box(p, P(64, 80), P(16, 14), 4), Box(p, P(64, 44), P(20, 14), 4))),
                    p => Min(Min(Seg(p, P(64, 70), P(64, 56), 2f), Circle(M(p), P(76, 94), 4)), Circle(M(p), P(112, 86), 3.5f)),
                    p => Box(p, P(64, 110), P(7, 9), 6), new[] { P(46, 10), P(57, 8), P(71, 8), P(82, 10) }, 8f);

                case "bolt_player": return Bolt(name, new C(0.3f, 1f, 0.92f));
                case "bolt_enemy": return Bolt(name, new C(1f, 0.2f, 0.25f));
                case "gem_xp": return Gem();

                case "ico_weapon": return Icon(name, p => Min(Min(Box(p, P(70, 80), P(40, 9), 4), Box(p, P(48, 72), P(26, 16), 6)),
                    Min(Box(p, P(40, 44), P(10, 22), 4, -18f), Box(p, P(110, 80), P(5, 14), 2))));
                case "ico_hull": return Icon(name, p => Sub(Poly(p, P(64, 114), P(100, 98), P(96, 52), P(64, 16), P(32, 52), P(28, 98)) - 3f,
                    Min(Seg(p, P(46, 72), P(64, 56), 4), Seg(p, P(64, 56), P(84, 84), 4))));
                case "ico_engine": return Icon(name, p => Min(Min(Box(p, P(64, 110), P(16, 8), 3), Sub(Poly(p, P(50, 102), P(78, 102), P(94, 60), P(34, 60)) - 2f, Seg(p, P(30, 82), P(98, 82), 3))),
                    Min(Flame(p, P(64, 40), 13, 6), Min(Flame(p, P(44, 44), 7, 22), Flame(p, P(84, 44), 7, 22)))));
                case "ico_module": return Icon(name, p =>
                {
                    float chip = Sub(Box(p, P(64, 64), P(30, 30), 6), Ring(Box(p, P(64, 64), P(16, 16), 3), 5));
                    float pins = 1e9f;
                    for (int i = 0; i < 3; i++)
                    {
                        float o = 48 + i * 16;
                        pins = Min(pins, Min(Seg(p, P(o, 94), P(o, 110), 4), Seg(p, P(o, 34), P(o, 18), 4)));
                        pins = Min(pins, Min(Seg(p, P(94, o), P(110, o), 4), Seg(p, P(34, o), P(18, o), 4)));
                    }
                    return Min(chip, pins);
                });
                case "ico_ship": return Icon(name, p => Poly(p, P(64, 118), P(80, 60), P(104, 22), P(64, 38), P(24, 22), P(48, 60)) - 3f);
            }
            throw new ArgumentException("Unknown art: " + name);
        }

        static float Flame(V2 p, V2 c, float r, float tipY) => SMin(Circle(p, c, r), Poly(p, P(c.x, tipY), P(c.x - r * 0.8f, c.y - 2f), P(c.x + r * 0.8f, c.y - 2f)), 3f);

        static V2[] Hex(V2 c, float r)
        {
            var v = new V2[6];
            for (int i = 0; i < 6; i++) v[i] = c + new V2(MathF.Cos((90f + 60f * i) * Deg), MathF.Sin((90f + 60f * i) * Deg)) * r;
            return v;
        }

        static float Warden(V2 p)
        {
            var c = P(64, 64);
            float d = Min(Circle(p, c, 28), Ring(Circle(p, c, 50), 10));
            for (int i = 0; i < 4; i++)
            {
                float a = (45f + 90f * i) * Deg;
                var dir = new V2(MathF.Cos(a), MathF.Sin(a));
                d = Min(d, Seg(p, c + dir * 24f, c + dir * 54f, 5f));
            }
            return Min(d, Box(p, P(64, 112), P(8, 12), 3));
        }

        static float WardenLights(V2 p)
        {
            var c = P(64, 64);
            float d = Circle(p, c, 11);
            for (int i = 0; i < 8; i++)
            {
                float a = (22.5f + 45f * i) * Deg;
                d = Min(d, Circle(p, c + new V2(MathF.Cos(a), MathF.Sin(a)) * 50f, 3f));
            }
            return Min(d, Circle(p, P(64, 122), 4));
        }

        static float Leviathan(V2 p)
        {
            float body = SMin(Box(p, P(64, 68), P(20, 50), 16), Poly(p, P(64, 98), P(124, 52), P(112, 20), P(64, 40), P(16, 20), P(4, 52)), 6f);
            return Sub(body, Poly(p, P(64, 106), P(55, 126), P(73, 126)));
        }

        /// <summary>Top-down craft pointing up. Shapes are authored in 128x128 units and rendered at any size.</summary>
        static Art Craft(string name, int size, Style st, Func<V2, float> hull, Func<V2, float> panels,
            Func<V2, float> accent, Func<V2, float> cockpit, V2[] engines, float engineR)
        {
            var a = new Art(name, size, size);
            float unit = 128f / size, ppu = size / 128f;
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    var p = new V2((x + 0.5f) * unit, (y + 0.5f) * unit);
                    int i = y * size + x;
                    float dh = hull(p);

                    if (engines != null)
                        foreach (var e in engines)
                        {
                            var q = p - e;
                            float qy = q.y < 0f ? q.y / 3f : q.y * 1.5f;
                            float r = MathF.Sqrt(q.x * q.x + qy * qy);
                            a.Over(i, st.glow.WithA(0.9f * MathF.Exp(-r / engineR)));
                            a.Over(i, C.White.WithA(0.8f * MathF.Exp(-r / (engineR * 0.3f))));
                        }

                    if (st.aura > 0f && dh > 0f) a.Over(i, st.glow.WithA(st.aura * MathF.Exp(-dh / 4f)));

                    float cov = Clamp01(0.5f - dh * ppu);
                    if (cov > 0f)
                    {
                        float shade = Clamp01(0.55f - (p.x - 64f) * 0.006f + (p.y - 64f) * 0.004f);
                        var col = C.Lerp(st.dark, st.hull, shade);
                        col = C.Lerp(col, st.hull * 1.25f, Clamp01(1f + dh / 4f) * 0.25f);
                        if (panels != null)
                        {
                            float pl = Clamp01(0.5f - (MathF.Abs(panels(p)) - 0.6f) * ppu);
                            col = C.Lerp(col, st.dark * 0.6f, pl * 0.8f);
                        }
                        col = C.Lerp(col, st.accent, Clamp01(1f - MathF.Abs(dh + 1.1f) / 0.7f) * st.rim);
                        if (cockpit != null)
                        {
                            float dc = cockpit(p);
                            float cc = Clamp01(0.5f - dc * ppu);
                            if (cc > 0f)
                            {
                                var glass = C.Lerp(st.cockpit * 0.55f, st.cockpit * 1.2f, Clamp01((p.y - 64f) / 60f + 0.3f));
                                glass = C.Lerp(glass, C.White, Clamp01(-dc / 3f) * 0.25f);
                                col = C.Lerp(col, glass, cc);
                            }
                        }
                        a.Over(i, col.WithA(cov));
                    }

                    if (accent != null)
                    {
                        float da = accent(p);
                        if (da > 0f) a.Over(i, st.accent.WithA(0.6f * MathF.Exp(-da / 2.5f)));
                        float ac = Clamp01(0.5f - da * ppu);
                        if (ac > 0f) a.Over(i, C.Lerp(st.accent, C.White, Clamp01(-da / 2f) * 0.6f).WithA(ac));
                    }
                }
            return a;
        }

        static Art Bolt(string name, C col)
        {
            var a = new Art(name, 64, 128);
            a.Paint(p =>
            {
                float d = Seg(p, P(32, 34), P(32, 94), 5f);
                float halo = 0.7f * MathF.Exp(-MathF.Max(0f, d) / 7f);
                float core = Clamp01(0.5f - d);
                var c = C.Lerp(col, C.White, core * 0.7f);
                return c.WithA(MathF.Max(halo, core));
            });
            return a;
        }

        static Art Gem()
        {
            var a = new Art("gem_xp", 64, 64);
            a.Paint(p =>
            {
                float d = Poly(p, P(32, 58), P(50, 32), P(32, 6), P(14, 32));
                float halo = 0.5f * MathF.Exp(-MathF.Max(0f, d) / 5f);
                var col = new C(0.3f, 0.95f, 0.88f);
                if (d > 0f) return col.WithA(halo);
                bool light = p.x < 32f == p.y > 32f;
                var face = light ? new C(0.6f, 1f, 0.95f) : new C(0.08f, 0.6f, 0.62f);
                return face.WithA(Clamp01(0.5f - d));
            });
            return a;
        }
    }
}
