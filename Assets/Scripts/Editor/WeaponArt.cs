using System;

namespace GamingApp.EditorTools
{
    /// <summary>Procedural weapon art: missiles, drones, orbs, tracers, effects and weapon icons.</summary>
    public static partial class UiArt
    {
        public static readonly string[] WeaponAll =
        {
            "missile_heavy", "missile_swarm", "bullet_ciws", "plasma_orb", "bolt_heavy", "drone_small", "drone_heavy",
            "fx_explosion", "fx_spark", "beam",
            "ico_w_blaster", "ico_w_laser", "ico_w_ciws", "ico_w_missile", "ico_w_swarm", "ico_w_plasma", "ico_w_drones", "ico_w_hdrones", "ico_sensor",
        };

        static Art MakeWeaponArt(string name)
        {
            switch (name)
            {
                case "missile_heavy": return Craft(name, 128, Tinted(Teal, new C(0.88f, 0.9f, 0.96f)),
                    p => MissileShape(p, 10f, 30f), p => Seg(p, P(64, 80), P(64, 44), 0f),
                    p => Box(p, P(64, 86), P(10, 2.2f), 1f), null, new[] { P(64, 32) }, 9f);
                case "missile_swarm": return Craft(name, 128, Tinted(new C(0.75f, 0.5f, 1f), new C(0.8f, 0.75f, 0.95f)),
                    p => MissileShape(p, 7f, 26f), null,
                    p => Circle(p, P(64, 96), 3f), null, new[] { P(64, 36) }, 8f);
                case "bullet_ciws": return Tracer();
                case "plasma_orb": return Orb(name, new C(0.55f, 1f, 0.85f), new C(0.15f, 0.75f, 0.7f));
                case "bolt_heavy": return Bolt(name, new C(0.45f, 0.62f, 1f));
                case "drone_small": return Craft(name, 128, Tinted(Teal, new C(0.82f, 0.88f, 0.98f)),
                    p => Min(Poly(p, P(64, 110), P(94, 62), P(64, 44), P(34, 62)), Circle(M(p), P(92, 70), 8)),
                    p => Seg(p, P(64, 96), P(64, 56), 0f),
                    p => Circle(p, P(64, 74), 6), null, new[] { P(64, 44) }, 7f);
                case "drone_heavy": return Craft(name, 256, Tinted(Blue, new C(0.8f, 0.85f, 0.97f)),
                    p => Min(Min(Poly(p, Hex(P(64, 64), 34)) - 3f, Box(M(p), P(100, 64), P(10, 26), 6)), Box(M(p), P(100, 100), P(3.5f, 14), 1.5f)),
                    p => Poly(p, Hex(P(64, 64), 20)),
                    p => Min(Circle(p, P(64, 64), 9), Circle(M(p), P(100, 40), 3)), null, new[] { P(52, 30), P(76, 30) }, 7f);
                case "fx_explosion": return Explosion();
                case "fx_spark": return Spark();
                case "beam": return Beam();

                case "ico_w_blaster": return Icon(name, p => Min(Seg(p, P(34, 34), P(84, 84), 10), Min(Seg(p, P(72, 30), P(98, 56), 3.5f), Seg(p, P(30, 72), P(56, 98), 3.5f))));
                case "ico_w_laser": return Icon(name, p => Min(Seg(p, P(18, 18), P(92, 92), 4.5f),
                    Min(Circle(p, P(98, 98), 11), Min(Seg(p, P(98, 74), P(98, 122), 2.5f), Seg(p, P(74, 98), P(122, 98), 2.5f)))));
                case "ico_w_ciws": return Icon(name, p => Min(Min(Box(p, P(46, 76), P(7, 34), 3), Box(p, P(64, 80), P(7, 38), 3)),
                    Min(Box(p, P(82, 76), P(7, 34), 3), Box(p, P(64, 30), P(30, 12), 5))));
                case "ico_w_missile": return Icon(name, p => MissileShape(Rot(p, P(64, 64), -45f), 11f, 26f));
                case "ico_w_swarm": return Icon(name, p => Min(SmallMissile(p, P(38, 52)), Min(SmallMissile(p, P(64, 70)), SmallMissile(p, P(90, 52)))));
                case "ico_w_plasma": return Icon(name, p => Min(Circle(p, P(64, 64), 22), Min(Ring(Circle(p, P(64, 64), 40), 7), Circle(p, P(100, 92), 7))));
                case "ico_w_drones": return Icon(name, DronesIcon);
                case "ico_sensor": return Icon(name, p =>
                {
                    var c = P(64, 22);
                    var d = p - c;
                    float ang = MathF.Atan2(d.y, d.x) / Deg;
                    float wedge = MathF.Max(40f - ang, ang - 140f) * d.Len * Deg;
                    float arcs = Min(Ring(Circle(p, c, 34), 8), Min(Ring(Circle(p, c, 56), 8), Ring(Circle(p, c, 78), 8)));
                    return Min(MathF.Max(arcs, wedge), Circle(p, c, 11));
                });
                case "ico_w_hdrones": return Icon(name, p => Min(Min(Poly(p, Hex(P(40, 58), 22)) - 2f, Poly(p, Hex(P(88, 58), 22)) - 2f),
                    Min(Box(p, P(40, 92), P(4, 12), 2), Box(p, P(88, 92), P(4, 12), 2))));
            }
            return MakeSystemArt(name);
        }

        static Style Tinted(C accent, C hull) => new Style
        {
            hull = hull, dark = hull * 0.35f, accent = accent, glow = accent, cockpit = accent, aura = 0.12f, rim = 0.5f,
        };

        static V2 Rot(V2 p, V2 c, float deg)
        {
            float cs = MathF.Cos(deg * Deg), sn = MathF.Sin(deg * Deg);
            var q = p - c;
            return c + new V2(q.x * cs + q.y * sn, -q.x * sn + q.y * cs);
        }

        static float MissileShape(V2 p, float w, float len)
        {
            float body = Box(p, P(64, 64), P(w, len), w * 0.9f);
            float nose = Poly(p, P(64, 64 + len + w * 2.2f), P(64 - w, 64 + len - w), P(64 + w, 64 + len - w));
            float fins = Poly(M(p), P(64 + w, 64 - len * 0.35f), P(64 + w * 2.6f, 64 - len - 2f), P(64 + w, 64 - len * 0.8f));
            return Min(SMin(body, nose, 2f), fins);
        }

        static float SmallMissile(V2 p, V2 c)
        {
            var q = p - c + P(64, 64);
            float body = Box(q, P(64, 60), P(6, 18), 5);
            float nose = Poly(q, P(64, 90), P(58, 76), P(70, 76));
            float fins = Poly(M(q), P(69, 50), P(78, 38), P(69, 44));
            return Min(SMin(body, nose, 2f), fins);
        }

        static float DronesIcon(V2 p)
        {
            float d = Circle(p, P(64, 64), 10);
            for (int i = 0; i < 4; i++)
            {
                float ang = (45f + 90f * i) * Deg;
                var cc = P(64, 64) + new V2(MathF.Cos(ang), MathF.Sin(ang)) * 38f;
                d = Min(d, Poly(p, cc + P(0, 14), cc + P(12, 0), cc + P(0, -14), cc + P(-12, 0)));
            }
            return d;
        }

        static Art Orb(string name, C core, C outer)
        {
            var a = new Art(name, 128, 128);
            var c = P(64, 64);
            a.Paint(p =>
            {
                var d = p - c;
                float r = d.Len;
                float swirl = 0.5f + 0.5f * MathF.Sin(MathF.Atan2(d.y, d.x) * 3f + r * 0.18f);
                float body = Clamp01(1f - r / 34f);
                float halo = 0.55f * MathF.Exp(-MathF.Max(0f, r - 26f) / 10f);
                var col = C.Lerp(outer, core, body * body);
                col = C.Lerp(col, C.White, MathF.Pow(Clamp01(1f - r / 16f), 1.5f));
                return col.WithA(Clamp01(MathF.Max(halo * (0.7f + 0.3f * swirl), Clamp01(34f - r))));
            });
            return a;
        }

        static Art Tracer()
        {
            var a = new Art("bullet_ciws", 32, 64);
            a.Paint(p =>
            {
                float d = Seg(p, P(16, 18), P(16, 50), 2.6f);
                var col = C.Lerp(new C(0.85f, 0.95f, 1f), C.White, Clamp01(-d));
                return col.WithA(MathF.Max(Clamp01(0.5f - d), 0.6f * MathF.Exp(-MathF.Max(0f, d) / 3.5f)));
            });
            return a;
        }

        static Art Explosion()
        {
            var a = new Art("fx_explosion", 256, 256);
            var c = P(128, 128);
            a.Paint(p =>
            {
                var d = p - c;
                float r = d.Len;
                float ring = MathF.Exp(-MathF.Pow((r - 104f) / 9f, 2f));
                float core = MathF.Pow(Clamp01(1f - r / 110f), 1.6f) * 0.75f;
                float spikes = 0.25f * MathF.Pow(Clamp01(1f - r / 120f), 0.7f) * (0.5f + 0.5f * MathF.Sin(MathF.Atan2(d.y, d.x) * 9f));
                return C.White.WithA(Clamp01(ring + core + spikes));
            });
            return a;
        }

        static Art Spark()
        {
            var a = new Art("fx_spark", 64, 64);
            var c = P(32, 32);
            a.Paint(p =>
            {
                var d = p - c;
                float star = MathF.Max(MathF.Exp(-MathF.Abs(d.x) / 1.6f) * Clamp01(1f - MathF.Abs(d.y) / 30f),
                                       MathF.Exp(-MathF.Abs(d.y) / 1.6f) * Clamp01(1f - MathF.Abs(d.x) / 30f));
                return C.White.WithA(Clamp01(MathF.Max(star, 0.6f * MathF.Exp(-d.Len / 7f))));
            });
            return a;
        }

        static Art Beam()
        {
            var a = new Art("beam", 32, 32);
            a.Paint(p =>
            {
                float dy = p.y - 16f;
                return C.White.WithA(Clamp01(MathF.Exp(-dy * dy / 50f) * 0.8f + MathF.Exp(-dy * dy / 5f)));
            });
            return a;
        }
    }
}
