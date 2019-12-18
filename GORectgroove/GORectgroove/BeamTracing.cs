using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace GORectgroove
{
    public class Point 
    {
        public double x;
        public double z;
    }
    public class Beam
    {
        public Point pointStart = new Point();
        public Point pointEnd = new Point();
        public int splitIndex;
        public int rayIndex;
        public double angle = 0;
        public double OPD = 0;
        public Complex amplitude;
        public bool bExit = false;
        public int reflectionTime = 0;
        public Beam(double xs, double zs, double xe, double ze, double angle0, double OPD0, Complex a0, int r0, int si0, int ri0, bool b0)
        {
            pointStart.x = xs; pointStart.z = zs;
            pointEnd.x = xe; pointEnd.z = ze;       
            angle = angle0;
            OPD = OPD0; amplitude = a0;
            reflectionTime = r0;
            splitIndex = si0;                                 //批次
            rayIndex = ri0;                                   //批次内编号，找相邻光线
            bExit = b0;
        }
    }
    public class BeamTracing
    {
        public List<Beam> beamList = new List<Beam>();
        public double width;
        public double depth;///
        public double incidenceAngle;               //与x轴正向夹角
        public int sampleRays;
        public double lambda;
        public int sampleAmplitude;                  //表面复振幅取样点数
        public Complex[] electricFieldAmplitude;
        public Complex refractiveIndex;
        public double sampleDx;                         //相邻光线之间的距离
        public void InitParameter(double w0, double d0, double a0, int s0, double l0, int sampleAmp, Complex refIndex)
        {
            width = w0; depth = d0; incidenceAngle = a0; sampleRays = s0; lambda = l0;
            sampleAmplitude = sampleAmp;
            electricFieldAmplitude = new Complex[sampleAmplitude];
            for (int i = 0; i < sampleAmplitude; i++)
                electricFieldAmplitude[i] = new Complex();
            refractiveIndex = refIndex;
            sampleDx = w0 / (sampleRays - 1) * Math.Abs(Math.Sin(incidenceAngle));
        }
        public void Intersection(ref double xe, ref double ze, double xs, double zs, double angle, ref bool bExit)
        {
            double x0, y0, x1, y1, x2, y2, x3, y3;
            double a, b, c;
            a = -Math.Sin(angle);
            b = Math.Cos(angle);
            c = a*xs + b*zs;
            x0 = -width / 2;
            x1 = width / 2;
            if (b != 0)
            {
                y0 = (c - a * x0) / b;
                y1 = (c - a * x1) / b;
            }
            else
            {
                y0 = double.MaxValue;
                y1 = double.MaxValue;            
            }
            y2 = 0;
            y3 = -depth;
            if (a != 0)
            {
                x2 = (c - b * y2) / a;
                x3 = (c - b * y3) / a;
            }
            else
            {
                x2 = double.MaxValue;
                x3 = double.MaxValue;
            }
            if (y0 > -depth && y0 < 0 && xs != -width / 2)
            {
                xe = x0; ze = y0; bExit = false; return;
            }
            if (y1 > -depth && y1 < 0 && xs != width / 2)
            {
                xe = x1; ze = y1; bExit = false; return;
            }
            if (x2 < width / 2+1e-9 && x2 > -width / 2-1e-9 && zs != 0)
            {
                xe = x2; ze = y2; bExit = true; return;
            }
            if (x3 < width / 2 && x3 > -width / 2 && zs != -depth)
            {
                xe = x3; ze = y3; bExit = false; return;
            }
            xe = xs; ze = zs; return;
        }
        public void InitRays()
        { 
            int i;
            double xs = 0, zs = 0, xe = 0, ze = 0, angle, OPD;
            Complex amplitude = new Complex(0,0);
            int reflectionTime, splitIndex, rayIndex;
            bool bExit = false;

            for (i = 0; i < sampleRays; i++)
            {
                zs = 0;  xs = (width-1e-10) / (sampleRays - 1) * (i - (sampleRays - 1) / 2);
                angle = incidenceAngle; amplitude = new Complex(1,0); reflectionTime = 0; splitIndex = 0;
                rayIndex = i;
                Intersection(ref xe, ref ze, xs, zs, angle, ref bExit);
                Beam beam = new Beam(xs, zs, xe, ze, angle, Math.Sqrt((xs - xe) * (xs - xe) + (zs - ze) * (zs - ze)) + xs * Math.Cos(angle), amplitude, reflectionTime, splitIndex, rayIndex, bExit);
                beamList.Add(beam);
            }
        }
        public double NormAngle(Point pe)
        {
            double normAngle = 0;
            if (Math.Abs(pe.x + width / 2) < 1e-12)
                normAngle = 0;
            else if (Math.Abs(pe.x - width / 2) < 1e-12)
                normAngle = Math.PI;
            else if (Math.Abs(pe.z + depth) < 1e-12)
                normAngle = Math.PI / 2;
            else
                normAngle = -Math.PI / 2;
            return normAngle;
        }
        public Complex ReflectionCoefficients(double incAngle, double normAngle, Complex refractiveIndex)
        {
            double n2, n1 = 1;
            double incNormAngle = incAngle - normAngle;
            if(incNormAngle < 0)
                incNormAngle = -incNormAngle;
            while(incNormAngle > Math.PI)
            {
                incNormAngle = incNormAngle-Math.PI;
            }
            if(incNormAngle >= Math.PI/2)
            {
                incNormAngle = Math.PI - incNormAngle;
            }
            double n, k;
            n = refractiveIndex.real; k = refractiveIndex.image;
            double u, v;
            double temp = n * n - k * k - Math.Sin(incNormAngle) * Math.Sin(incNormAngle);
            u = Math.Sqrt(1.0 / 2 * (temp + Math.Sqrt(temp * temp + 4 * n * n * k * k)));
            v = Math.Sqrt(1.0 / 2 * (-temp + Math.Sqrt(temp * temp + 4 * n * n * k * k)));
            Complex cp = new Complex(Math.Cos(incNormAngle), 0);
            Complex ct = new Complex(u,v);
            Complex result = (cp - ct) / (cp + ct);
            return result;
        }
        public void AmplitudeUpdate(List<Beam> lb)
        {
            double xe0, xe1, x, opd, phase;
            int j, i0, i1, i2;         //振幅数组索引
            double deltaX = width/(sampleAmplitude - 1);
            double tiltFactor;
            Complex amp;
            double ampmod0, ampmod1, ampmod;
            double phase0, phase1, phasem;
            for (int i = 0; i < lb.Count - 1; i++)
            {
                Beam beam0 = lb[i];
                Beam beam1 = lb[i+1];
                xe0 = beam0.pointEnd.x;
                xe1 = beam1.pointEnd.x;
                tiltFactor = sampleDx/Math.Sin(beam0.angle);
                if (beam0.rayIndex + 1 == beam1.rayIndex && Math.Abs(xe0 - xe1) - Math.Abs(tiltFactor) < 1e-9)
                {
                    if (xe0 < xe1)
                    {
                        i0 = Convert.ToInt32(Math.Ceiling((xe0 + width / 2) / deltaX));
                        i1 = Convert.ToInt32(Math.Floor((xe1 + width / 2) / deltaX));
                        ampmod0 = beam0.amplitude.Mod();
                        ampmod1 = beam1.amplitude.Mod();
                        phase0 = beam0.amplitude.GetAngle();
                        phase1 = beam1.amplitude.GetAngle();
                        if (phase1 > phase0 && phase1 - Math.PI > phase0)
                        {
                            while (phase1 > phase0 && phase1 - Math.PI > phase0)
                            {
                                phase1 = phase1 - 2 * Math.PI;
                            }
                        }
                        else if (phase0 > phase1 && phase0 - Math.PI > phase1)
                        {
                            while (phase0 > phase1 && phase0 - Math.PI > phase1)
                            {
                                phase0 = phase0 - 2 * Math.PI;
                            }
                        }
                        for (j = i0; j <= i1; j++)
                        {
                            x = j * deltaX - width / 2;
                            opd = (xe1 - x) / (xe1 - xe0) * beam0.OPD + (x - xe0) / (xe1 - xe0) * beam1.OPD;
                            ampmod = (xe1 - x) / (xe1 - xe0) * ampmod0 + (x - xe0) / (xe1 - xe0) * ampmod1;
                            phasem = (xe1 - x) / (xe1 - xe0) * phase0 + (x - xe0) / (xe1 - xe0) * phase1;
                            phase = 2 * Math.PI * opd / lambda;
                            electricFieldAmplitude[j] = electricFieldAmplitude[j] + (new Complex(Math.Cos(phasem) * ampmod, Math.Sin(phasem) * ampmod)) * (new Complex(Math.Cos(phase), Math.Sin(phase)));
                        }
                    }
                    else
                    {
                        i0 = Convert.ToInt32(Math.Floor((xe0 + width / 2) / deltaX));
                        i1 = Convert.ToInt32(Math.Ceiling((xe1 + width / 2) / deltaX));
                        ampmod0 = beam0.amplitude.Mod();
                        ampmod1 = beam1.amplitude.Mod();
                        phase0 = beam0.amplitude.GetAngle();
                        phase1 = beam1.amplitude.GetAngle();
                        if (phase1 > phase0 && phase1 - Math.PI > phase0)
                        {
                            while (phase1 > phase0 && phase1 - Math.PI > phase0)
                            {
                                phase1 = phase1 - 2 * Math.PI;
                            }
                        }
                        else if (phase0 > phase1 && phase0 - Math.PI > phase1)
                        {
                            while (phase0 > phase1 && phase0 - Math.PI > phase1)
                            {
                                phase0 = phase0 - 2 * Math.PI;
                            }
                        }
                        for (j = i0; j >= i1; j--)
                        {
                            x = j * deltaX - width / 2;
                            opd = (xe1 - x) / (xe1 - xe0) * beam0.OPD + (x - xe0) / (xe1 - xe0) * beam1.OPD;
                            phase = -2 * Math.PI * opd / lambda;
                            ampmod = (xe1 - x) / (xe1 - xe0) * ampmod0 + (x - xe0) / (xe1 - xe0) * ampmod1;
                            phasem = (xe1 - x) / (xe1 - xe0) * phase0 + (x - xe0) / (xe1 - xe0) * phase1;
                            phase = 2 * Math.PI * opd / lambda;
                            electricFieldAmplitude[j] = electricFieldAmplitude[j] + (new Complex(Math.Cos(phasem) * ampmod, Math.Sin(phasem) * ampmod)) * (new Complex(Math.Cos(phase), Math.Sin(phase)));
                        }

                    }
                }
            }
        }
        public void OutputAmplitude(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            for (int i = 0; i < electricFieldAmplitude.Length; i++)
            {
                sw.WriteLine(electricFieldAmplitude[i].real.ToString() + "    " + electricFieldAmplitude[i].image.ToString());
            }
            sw.Close();
            fs.Close();        
        }
        public Complex[] BeamTrace()
        {
            List<Beam> beamListUpdate = new List<Beam>();
            List<Beam> beamExitList = new List<Beam>();
            double xs = 0, zs = 0, xe = 0, ze = 0, angle, OPD;
            Complex amplitude = new Complex(0,0);
            int reflectionTime, splitIndex, rayIndex;
            bool bExit = false;
            Complex reflectionCoefficients;
            double deltaX = width / (sampleAmplitude - 1);
            double x;
            for (int i = 0; i < sampleAmplitude; i++)
            {
                x = i * deltaX - width / 2;
            
            
            }


            while (true)
            {
                if (beamList.Count == 0)
                    break;
                for (int i = 0; i < beamList.Count; i++)
                {
                    if (i == 801)
                        i = i;
                    Beam beam = beamList[i];
                    double normAngle = NormAngle(beam.pointEnd);
                    angle = 2*normAngle - beam.angle - Math.PI;
                    reflectionTime = beam.reflectionTime + 1; splitIndex = 0;
                    reflectionCoefficients = ReflectionCoefficients(beam.angle, normAngle, refractiveIndex);
                    amplitude = beam.amplitude * reflectionCoefficients; rayIndex = beam.rayIndex;
                    xs = beam.pointEnd.x; zs = beam.pointEnd.z;
                    Intersection(ref xe, ref ze, xs, zs, angle, ref bExit);
                    Beam beamUpdate = new Beam(xs, zs, xe, ze, angle, beam.OPD + Math.Sqrt((xs - xe) * (xs - xe) + (zs - ze) * (zs - ze)), amplitude, reflectionTime, splitIndex, rayIndex, bExit);
                    if (bExit == false)
                    {
                        if(beamUpdate.amplitude.Mod() > 1e-6)
                            beamListUpdate.Add(beamUpdate);
                    }
                    else
                        beamExitList.Add(beamUpdate);
                }
                AmplitudeUpdate(beamExitList);
                beamList.Clear();
                for (int i = 0; i < beamListUpdate.Count; i++)
                    beamList.Add(beamListUpdate[i]);
                beamListUpdate.Clear();
                beamExitList.Clear();
            }
            //OutputAmplitude();
            return electricFieldAmplitude;
        }


    }
}
