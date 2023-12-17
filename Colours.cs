using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Remaster
{
    public static class Colours
    {
        static LinearGradientBrush Gradient;
        public static LinearGradientBrush BlueGradient()
        {
            Gradient = new LinearGradientBrush();
            Gradient.StartPoint = new Point(0, 0);
            Gradient.EndPoint = new Point(1, 1);
            Gradient.GradientStops.Add(new GradientStop(Color.FromArgb(255, 173, 216, 230), 0.0));  
            Gradient.GradientStops.Add(new GradientStop(Color.FromArgb(255, 135, 206, 235), 0.25)); 
            Gradient.GradientStops.Add(new GradientStop(Color.FromArgb(255, 100, 149, 237), 0.5)); 

            return Gradient;
        }
        public static LinearGradientBrush RedGradient()
        {
            LinearGradientBrush Gradient = new LinearGradientBrush();
            Gradient.StartPoint = new Point(0, 0);
            Gradient.EndPoint = new Point(1, 1);
            Gradient.GradientStops.Add(new GradientStop(Colors.LightSalmon, 0.0));
            Gradient.GradientStops.Add(new GradientStop(Colors.Salmon, 0.25));
            Gradient.GradientStops.Add(new GradientStop(Colors.Crimson, 0.5));
            Gradient.GradientStops.Add(new GradientStop(Colors.Firebrick, 0.75));

            return Gradient;
        }
        public static LinearGradientBrush VioletGradient()
        {
            LinearGradientBrush gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0, 0);
            gradient.EndPoint = new Point(1, 1);
            gradient.GradientStops.Add(new GradientStop(Colors.Lavender, 0.0));
            gradient.GradientStops.Add(new GradientStop(Colors.Thistle, 0.25));
            gradient.GradientStops.Add(new GradientStop(Colors.Orchid, 0.5));
            gradient.GradientStops.Add(new GradientStop(Colors.DarkViolet, 0.75));
            gradient.GradientStops.Add(new GradientStop(Colors.Indigo, 1.0)); 

            return gradient;
        }
        public static LinearGradientBrush GreenGradient()
        {
            LinearGradientBrush gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0, 0);
            gradient.EndPoint = new Point(1, 1);
            gradient.GradientStops.Add(new GradientStop(Colors.LightGreen, 0.0));
            gradient.GradientStops.Add(new GradientStop(Colors.LimeGreen, 0.25));
            gradient.GradientStops.Add(new GradientStop(Colors.ForestGreen, 0.5));
            gradient.GradientStops.Add(new GradientStop(Colors.DarkGreen, 0.75));
            gradient.GradientStops.Add(new GradientStop(Colors.Green, 1.0));

            return gradient;
        }
        public static LinearGradientBrush OrangeGradient()
        {
            LinearGradientBrush gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0, 0);
            gradient.EndPoint = new Point(1, 1);
            gradient.GradientStops.Add(new GradientStop(Colors.LightSalmon, 0.0));
            gradient.GradientStops.Add(new GradientStop(Colors.Coral, 0.25));
            gradient.GradientStops.Add(new GradientStop(Colors.DarkOrange, 0.5));
            gradient.GradientStops.Add(new GradientStop(Colors.OrangeRed, 0.75));
            gradient.GradientStops.Add(new GradientStop(Colors.DarkRed, 1.0));

            return gradient;
        }
        public static LinearGradientBrush GreenGradientBrush()
        {
            LinearGradientBrush gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0, 0);
            gradient.EndPoint = new Point(1, 1);
            gradient.GradientStops.Add(new GradientStop(Colors.LightGreen, 0.0));
            gradient.GradientStops.Add(new GradientStop(Colors.Green, 0.5));
            gradient.GradientStops.Add(new GradientStop(Colors.DarkGreen, 1.0));

            return gradient;
        }
    }
}
