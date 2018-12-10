using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace masktest
{
    class Program
    {
        public static double[][] mask1 = new double[][]
        {
            new double[] {1,2,1},
            new double[] {2,4,2},
            new double[] {1,2,1}
        };

        public static double[][] mask2 = new double[][]
        {
            new double[] {1, 4, 6, 4,1,},
            new double[] {4,16,24,16,4,},
            new double[] {6,24,36,24,6,},
            new double[] {4,16,24,16,4,},
            new double[] {1, 4, 6, 4,1,},
        };

        public static double[][] mask4 = new double[][]
        {
            new double[] {1,2, 4,2,1,},
            new double[] {2,4, 8,4,2,},
            new double[] {4,8,16,8,4,},
            new double[] {2,4, 8,4,2,},
            new double[] {1,2, 4,2,1,},
        };

        public static double[][] data = new double[][]
        {
            new double[] {1,2,3,25,4,3,3,4,3,3},
            new double[] {1,2,3,5,4,3,3,42,3,3},
            new double[] {1,2,3,1,4,3,3,2,3,3},
            new double[] {1,2,3,5,4,3,3,42,3,3},
            new double[] {1,2,3,2,5,3,3,42,3,3},
            new double[] {1,2,3,5,4,3,3,2,3,3},
            new double[] {1,2,3,4,4,3,3,42,3,3},
            new double[] {1,2,3,2,5,3,3,42,3,3},
        };

        static void Main(string[] args)
        {
            Console.WriteLine("original data:");
            var d = CloneArray(data);
            DumpMask(d);
            var d1 = DoMask(d, CloneArray(mask1), new Number() { numerator = 1, denominator = 16 });
            var d2 = DoMask(d1, CloneArray(mask1), new Number() { numerator = 1, denominator = 16 });
            Console.WriteLine("after mask1 twice:");
            DumpMask(d2);

            var d3 = DoMask(d, CloneArray(mask2), new Number() { numerator = 1, denominator = 256 });
            Console.WriteLine("after mask2 :");
            DumpMask(d3);

            var newmask = new double[][]
            {
                new double[] {0,0,0,0,0},
                new double[] {0,0,0,0,0},
                new double[] {0,0,0,0,0},
                new double[] {0,0,0,0,0},
                new double[] {0,0,0,0,0},
            };

            var nd = CloneArray(newmask);
            for (int x=0;x<=2;x++)
            {
                for (int y=0;y<=2;y++)
                {
                    AddMask(nd, CloneArray(mask1), x, y);
                }
            }

            Console.WriteLine("Calculated mask:");
            DumpMask(nd);
            Console.WriteLine("Sum value:" + GetArrayInfos(nd).Select(x => x.value.numerator).Sum());
            Console.ReadLine();
        }

        static void AddMask(Number[][] data, Number[][] add, int addx, int addy)
        {
            foreach (var addv in GetArrayInfos(add))
            {
                var datav = new ArrayDataInfo<Number>()
                {
                    data = data,
                    x = addv.x + addx,
                    y = addv.y + addy,
                };
                datav.value = datav.value.Plus(addv.value);
            }
        }

        static void DumpMask(Number[][] data)
        {
            foreach (var xd in GetArrayInfos(data).GroupBy(d => d.y))
            {
                foreach (var d in xd.OrderBy(x => x.x))
                {
                    Console.Write(string.Format("{0}/{1}\t", d.value.numerator, d.value.denominator));
                }
                Console.WriteLine();
            }
        }

        static Number[][] DoMask(Number[][] data, Number[][] mask, Number multiply)
        {
            var r = CloneArray(data);
           foreach (var d in GetArrayInfos(data))
            {
                var rd = new ArrayDataInfo<Number>()
                {
                    data = r,
                    x = d.x,
                    y = d.y
                };

                var values = GetArrayInfos(mask).Select(
                    m =>
                    {
                        var dm = new ArrayDataInfo<Number>()
                        {
                            data = data,
                            x = m.x + d.x,
                            y = m.y + d.y,
                        };
                        return m.value.Multiply(dm.value);
                    }).ToArray();
                var n = new Number() { numerator = 0, denominator = 1 };
                foreach (var v in values)
                {
                    n = n.Plus(v);
                }
                rd.value = n.Multiply(multiply);
            }

            return r;
        }

        static IEnumerable<ArrayDataInfo<T>> GetArrayInfos<T>(T[][] data)
        {
            for (int y = 0; y < data.Length; y++)
            {
                var xdata = data[y];
                for (int x = 0; x < xdata.Length; x++)
                {
                    yield return new ArrayDataInfo<T>()
                    {
                        data = data,
                        x = x,
                        y = y,
                    };
                }
            }
        }

        static Number[][] CloneArray(double[][] data)
        {
            Number[][] r = new Number[data.Length][];

            for(int y=0;y<data.Length;y++)
            {
                r[y] = data[y].Select(d => new Number() { numerator = d, denominator = 1 }).ToArray();
            }

            return r;
        }

        static Number[][] CloneArray(Number[][] data)
        {
            Number[][] r = new Number[data.Length][];

            for (int y = 0; y < data.Length; y++)
            {
                r[y] = data[y].Select(d => new Number() { numerator = d.numerator, denominator = d.denominator }).ToArray();
            }

            return r;
        }
    }

    public class Number
    {
        public double numerator;
        public double denominator;

        public Number Multiply(Number n)
        {
            if (n == null)
            {
                return new Number()
                {
                    numerator = 0,
                    denominator = 1,
                };
            }
            else
            {
                return new Number()
                {
                    numerator = this.numerator * n.numerator,
                    denominator = this.denominator * n.denominator,
                };
            }
        }

        public Number Div(Number n)
        {
            return new Number()
            {
                numerator = this.numerator * n.denominator,
                denominator = this.denominator * n.numerator,
            };
        }

        public Number Plus(Number n)
        {
            if (n == null)
            {
                return new Number()
                {
                    numerator = 0,
                    denominator = 1,
                };
            }

            if (n.denominator == this.denominator)
            {
                return new Number()
                {
                    numerator = this.numerator + n.numerator,
                    denominator = this.denominator,
                };
            }
            else
            {
                return new Number()
                {
                    denominator = this.denominator * n.denominator,
                    numerator = this.numerator * n.denominator + n.numerator * denominator,
                };
            }
        }
    }

    public class ArrayDataInfo<T>
    {
        public T[][] data { get; set; }

        public int x { get; set; } = 0;
        public int y { get; set; } = 0;

        public T value
        {
            get
            {
                if (y >= data.Length)
                    return default(T);
                if (x >= data[y].Length)
                    return default(T);
                return data[y][x];
            }
            set
            {
                if (y >= data.Length)
                    return;
                if (x >= data[y].Length)
                    return;
                data[y][x] = value;
            }
        }
    }
}
