using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace RationalNumberProject
{
    public class Rational : IComparable<Rational>, IFormattable
    {
        public long Numerator { get; private set; }

        public long Denominator { get; private set; }

        public Rational(long numerator, long denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
            Reduce();
        }

        void Reduce()
        {
            long gcd = Math.Abs(GCD(Numerator, Denominator)) * Math.Sign(Denominator);
            Numerator /= gcd;
            Denominator /= gcd;
        }

        public static long GCD(long a, long b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        public Rational(long numerator)
        {
            Numerator = numerator;
            Denominator = 1;
        }

        public static long LCM(long a, long b)
        {
            return a / GCD(a, b) * b;
        }

        public static Rational operator -(Rational a)
        {
            return new Rational(-a.Numerator, a.Denominator);
        }

        public static Rational operator +(Rational a, Rational b)
        {
            long lcm = LCM(a.Denominator, b.Denominator);
            return new Rational(lcm / a.Denominator * a.Numerator +
                                lcm / b.Denominator * b.Numerator, lcm);
        }

        public static Rational operator -(Rational a, Rational b)
        {
            return a + (-b);
        }

        public static Rational operator ++(Rational a)
        {
            a += 1;
            return a;
        }

        public static Rational operator --(Rational a)
        {
            a -= 1;
            return a;
        }

        public static Rational operator *(Rational a, Rational b)
        {
            Rational r1 = new Rational(a.Numerator, b.Denominator);
            Rational r2 = new Rational(b.Numerator, a.Denominator);
            return new Rational(r1.Numerator * r2.Numerator, r1.Denominator * r2.Denominator);
        }

        public Rational Flip()
        {
            return new Rational(Denominator, Numerator);
        }

        public static Rational operator /(Rational a, Rational b)
        {
            return a * b.Flip();
        }

        public static Rational Abs(Rational a)
        {
            return new Rational(Math.Abs(a.Numerator), a.Denominator);
        }

        public static Rational Pow(Rational a, int n)
        {
            if (n < 0) return Pow(a.Flip(), -n);
            if (n == 0) return 1;
            if (n % 2 == 1) return Pow(a, n - 1) * a;
            Rational t = Pow(a, n / 2);
            return t * t;
        }
        

        override public int GetHashCode()
        {
            return HashCode.Combine(Numerator, Denominator);
        }

        public long IntPart()
        {
            return Numerator / Denominator;
        }

        public Rational FractPart()
        {
            return new Rational(Numerator % Denominator, Denominator);
        }

        int CompareAbs(Rational a, Rational b)
        {
            long d1 = a.IntPart();
            long d2 = b.IntPart();
            int t = d1.CompareTo(d2);
            if (t != 0) return t;
            Rational r1 = a.FractPart();
            if (r1 > 0) r1 = r1.Flip(); else return -1;
            Rational r2 = b.FractPart();
            if (r2 > 0) r2 = r2.Flip(); else return 1;
            return CompareAbs(r2, r1);
        }

        public int Sign()
        {
            return Math.Sign(Numerator);
        }

        public bool Equals(Rational other)
        {
            return Numerator == other.Numerator && Denominator == other.Denominator;
        }

        public override bool Equals(object obj)
        {
            return obj is Rational && Equals((Rational)obj);
        }

        public int CompareTo(Rational other)
        {
            if (Equals(other)) return 0;
            int t = Sign().CompareTo(other.Sign());
            if (t != 0) return t;
            if (Sign() == 1) return CompareAbs(Abs(this), Abs(other));
            return CompareAbs(Abs(other), Abs(this));
        }

        public static bool operator ==(Rational a, Rational b)
        {
            return Equals(a,b);
        }

        public static bool operator !=(Rational a, Rational b)
        {
            return !Equals(a,b);
        }

        public static bool operator >(Rational a, Rational b)
        {
            return a.CompareTo(b) == 1;
        }

        public static bool operator <(Rational a, Rational b)
        {
            return a.CompareTo(b) == -1;
        }

        public static bool operator >=(Rational a, Rational b)
        {
            return a.CompareTo(b) != -1;
        }

        public static bool operator <=(Rational a, Rational b)
        {
            return a.CompareTo(b) != 1;
        }

        static public Rational Parse(string s)
        {
            if (!TryParse(s, out Rational result)) throw new FormatException("Error parse");
            return result;
        }

        static public Rational Parse(string format, string s)
        {
            if (!TryParse(s, format, out Rational result)) throw new FormatException("Error parse");
            return result;
        }

        static public bool TryParse(string s, out Rational result)
        {
            return TryParse(s, "I", out result) || TryParse(s, "D", out result);
        }

        static public bool TryParse(string s, string format, out Rational result)
        {
            s = s.Trim();
            result = default;
            switch (format)
            {
                case "s":
                    {
                        string pattern = @"^(-?\d+)\s*[/]\s*(\d+)\z";
                        if (!Regex.IsMatch(s, pattern)) return false;
                        Match m = Regex.Match(s, pattern);
                        if (!long.TryParse(m.Groups[1].Value, out long value1)) return false;
                        if (!long.TryParse(m.Groups[2].Value, out long value2)) return false;
                        if (value2 == 0) return false;
                        result = new Rational(value1, value2);
                        return true;
                    }
                case "S":
                    {
                        if (!long.TryParse(s, out long value1)) return TryParse(s, "s", out result);
                        result = new Rational(value1);
                        return true;
                    }
                case "i":
                    {
                        string pattern = @"^(-?)(\d+)\s+(\d+)\s*[/]\s*(\d+)\z";
                        if (!Regex.IsMatch(s, pattern)) return false;
                        Match m = Regex.Match(s, pattern);
                        bool isPositive = m.Groups[1].Value != "-";
                        if (!long.TryParse(m.Groups[2].Value, out long value1)) return false;
                        if (!long.TryParse(m.Groups[3].Value, out long value2)) return false;
                        if (!long.TryParse(m.Groups[4].Value, out long value3)) return false;
                        result = value1 + new Rational(value2, value3);
                        if (!isPositive) result = -result;
                        return true;
                    }
                case "I":
                    {
                        return TryParse(s, "S", out result) || TryParse(s, "i", out result);
                    }
                case "d":
                    {
                        if (!decimal.TryParse(s, out decimal value)) return false;
                        result = value;
                        return true;
                    }
                case "D":
                    {
                        if (Rational.TryParse(s, "d", out result)) return true;
                        string pattern = @"^(-?)(\d+)[.](\d*)[(](\d+)[)]\z";
                        if (!Regex.IsMatch(s, pattern)) return false;
                        Match m = Regex.Match(s, pattern);
                        bool isPositive = m.Groups[1].Value != "-";
                        if (!long.TryParse(m.Groups[2].Value, out long intPart)) return false;
                        string sa = m.Groups[3].Value + m.Groups[4].Value;
                        string sb = m.Groups[3].Value;
                        long a, b;
                        if (!long.TryParse(sa, out a)) return false;
                        if (sb == "") b = 0;
                        else if (!long.TryParse(sb, out b)) return false;
                        int n = sa.Length;
                        int k = sa.Length - sb.Length;
                        if (n > 18) return false;
                        result = intPart + new Rational(a - b,
                            long.Parse(new string('9', k) + new string('0', n - k)));
                        if (!isPositive) result = -result;
                        return true;
                    }
                default:
                    throw new FormatException(string.Concat("Unknown format"));
            }
        }

        override public string ToString()
        {
            return ToString("S");
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return ToString(format);
        }

        public string ToString(string format)
        {
            if (format == null) format = "S";
            switch (format)
            {
                case "s": 
                    return string.Concat(Numerator, "/", Denominator);
                case "S":
                    if (Denominator == 1) return Numerator.ToString();
                    return ToString("s");
                case "i":
                    string res = string.Concat(Math.Abs(Numerator / Denominator), " ",
                        Math.Abs(Numerator % Denominator), "/", Denominator);
                    if (Numerator < 0) res = "-" + res;
                    return res;
                case "I":
                    if (Numerator / Denominator == 0 || Numerator % Denominator == 0) return ToString("S");
                    else return string.Concat(Numerator / Denominator, " ", Math.Abs(Numerator %
                        Denominator), "/", Denominator);
                case "d":
                    return ((decimal)Numerator / Denominator).ToString();
                case "D":
                    return RationalToString(Numerator, Denominator);
                default:
                    throw new FormatException("Unknown format");
            }
        }

        private static string RationalToString(long numerator, long denominator)
        {
            Dictionary<long, int> d = new Dictionary<long, int>();
            string res = "";
            if (numerator < 0)
            {
                res = "-";
                numerator = -numerator;
            }
            for (int i = 0; i < 100; i++)
            {
                res += numerator / denominator;
                long r = numerator % denominator;
                if (r == 0) return res;
                if (d.TryGetValue(r, out int ind)) return res.Insert(ind, "(") + ')';
                if (i == 0) res += ".";
                d.Add(r, res.Length);
                numerator = r * 10;
            }
            return res + "...";
        }

        public static implicit operator Rational(long value)
        {
            return new Rational(value);
        }

        public static implicit operator Rational(decimal value)
        {
            return new Rational((long)(value * (int)1e9), (int)1e9);
        }

        public static explicit operator decimal(Rational value)
        {
            return (decimal)value.Numerator / value.Denominator;
        }

        public static explicit operator long(Rational value)
        {
            return value.Numerator / value.Denominator;
        }
    }
}
