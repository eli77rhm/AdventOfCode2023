using Microsoft.Z3;

namespace Aoc23
{
    public class Day24 : AllDays
    {
        public Day24() : base("Day24")
        {
        }

        public override void ExecutePart1()
        {
            double minX = 200000000000000;
            double maxX = 400000000000000;
            double minY = 200000000000000;
            double maxY = 400000000000000;

            var hailstones = Parse();
            int count = CountIntersections(hailstones, minX, maxX, minY, maxY);
            Console.WriteLine($"Number of intersections in the test area: {count}");
        }

        public override void ExecutePart2()
        {
            var hailstones = Parse();

            using (Context context = new Context())
            {
                IntExpr colliderX = context.MkIntConst("colliderX");
                IntExpr colliderY = context.MkIntConst("colliderY");
                IntExpr colliderZ = context.MkIntConst("colliderZ");
                IntExpr colliderVx = context.MkIntConst("colliderVx");
                IntExpr colliderVy = context.MkIntConst("colliderVy");
                IntExpr colliderVz = context.MkIntConst("colliderVz");

                Solver s = context.MkSolver();
                int count = 0;
                foreach (Hailstone hailstone in hailstones)
                {
                    IntExpr t = context.MkIntConst($"t_{hailstone.X}_{hailstone.Y}_{hailstone.Z}");

                    IntExpr hailstoneX = context.MkInt(hailstone.X.ToString());
                    IntExpr hailstoneY = context.MkInt(hailstone.Y.ToString());
                    IntExpr hailstoneZ = context.MkInt(hailstone.Z.ToString());
                    IntExpr hailstoneVx = context.MkInt(hailstone.Vx.ToString());
                    IntExpr hailstoneVy = context.MkInt(hailstone.Vy.ToString());
                    IntExpr hailstoneVz = context.MkInt(hailstone.Vz.ToString());

                    s.Assert(t >= 0);

                    s.Assert(context.MkEq(context.MkAdd(hailstoneX, context.MkMul(hailstoneVx, t)), colliderX + colliderVx * t));
                    s.Assert(context.MkEq(context.MkAdd(hailstoneY, context.MkMul(hailstoneVy, t)), colliderY + colliderVy * t));
                    s.Assert(context.MkEq(context.MkAdd(hailstoneZ, context.MkMul(hailstoneVz, t)), colliderZ + colliderVz * t));

                    count++;
                    if (count >= 3) break;
                }

                if (s.Check() == Status.SATISFIABLE)
                {
                    Console.WriteLine($"Intersection found at: x = {s.Model.Evaluate(colliderX)}, y = {s.Model.Evaluate(colliderY)} , z = {s.Model.Evaluate(colliderZ)}");
                    Console.WriteLine($"Intersection velocity : Vx = {s.Model.Evaluate(colliderVx)}, Vy = {s.Model.Evaluate(colliderVy)} , Vz = {s.Model.Evaluate(colliderVz)}");
                }
                else
                {
                    Console.WriteLine("No intersection found within the constraints.");
                }

                var result = Convert.ToInt64(s.Model.Evaluate(colliderX).ToString()) +
                             Convert.ToInt64(s.Model.Evaluate(colliderY).ToString()) +
                             Convert.ToInt64(s.Model.Evaluate(colliderZ).ToString());

                Console.WriteLine($"Result : {result}");
            }
        }

        private List<Hailstone> Parse()
        {
            var hailstones = new List<Hailstone>();
            foreach (var line in Lines)
            {
                var parts = line.Trim().Split(new[] { ' ', ',', '@' }, StringSplitOptions.RemoveEmptyEntries);
                double px = double.Parse(parts[0]);
                double py = double.Parse(parts[1]);
                double pz = double.Parse(parts[2]);
                double vx = double.Parse(parts[3]);
                double vy = double.Parse(parts[4]);
                double vz = double.Parse(parts[5]);
                hailstones.Add(new Hailstone(px, py, pz, vx, vy, vz));
            }
            return hailstones;
        }

        public class Hailstone
        {
            public double X, Y, Z;
            public double Vx, Vy, Vz;
            public double a, b, c;

            public Hailstone(double x, double y, double z, double vx, double vy, double vz)
            {
                X = x;
                Y = y;
                Z = z;
                Vx = vx;
                Vy = vy;
                Vz = vz;

                double m = Vy / Vx;
                a = m * -1;
                b = 1;
                c = m * X - Y;
            }

            public (bool, double) TimeAtPosition(double intersectionX, double intersectionY)
            {
                var t1 = (intersectionX - X) / Vx;
                var t2 = (intersectionY - Y) / Vy;

                if (Math.Abs(t1 - t2) > 1e-6)
                    return (false, t1);
                else
                    return (true, t1);
            }
        }

        private int CountIntersections(List<Hailstone> hailstones, double minX, double maxX, double minY, double maxY)
        {
            int intersections = 0;

            for (int i = 0; i < hailstones.Count; i++)
            {
                for (int j = i + 1; j < hailstones.Count; j++)
                {
                    var (t, x, y) = FindIntersection(hailstones[i], hailstones[j]);
                    if (t && x >= minX && x <= maxX && y >= minY && y <= maxY)
                    {
                        Console.WriteLine($" t:{t} , ({x},{y})");
                        intersections++;
                    }
                }
            }

            return intersections;
        }

        private (bool, double, double) FindIntersection(Hailstone p1, Hailstone p2)
        {
            var belowEquation = p1.a * p2.b - p2.a * p1.b;

            if (belowEquation == 0)
            {
                //parallel lines
                return (false, 0, 0);
            }

            var intersectionX = (p1.b * p2.c - p2.b * p1.c) / belowEquation;
            var intersectionY = (p1.c * p2.a - p2.c * p1.a) / belowEquation;

            (bool isPossible1, double time1) = p1.TimeAtPosition(intersectionX, intersectionY);
            (bool isPossible2, double time2) = p2.TimeAtPosition(intersectionX, intersectionY);

            //if (!isPossible1 || !isPossible2 || Math.Abs(time1 - time2) > 1e-6)
            //{
            //    return (false, 0, 0);
            //}

            if (time1 >= 0 && time2 >= 0)
            {
                return (true, intersectionX, intersectionY);
            }
            else
            {
                return (false, 0, 0); //in the past
            }

        }
    }
}
