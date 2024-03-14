//--------------------------------------------------------------------------------------------------------------------
// <copyright file="FunctionSeries.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Represents a line series that generates its dataset from a function.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GraphPlotter.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Represents a line series that generates its dataset from a function.
    /// </summary>
    /// <remarks>Define <code>f(x)</code> and make a plot on the range <code>[x0,x1]</code> or define <code>x(t)</code> and <code>y(t)</code> and make a plot on the range <code>[t0,t1]</code>.</remarks>
    public class FunctionSeries
    {
        public Func<float, float> function;
        public String title;
        /// <summary>
        /// Initializes a new instance of the <see cref = "FunctionSeries" /> class.
        /// </summary>
        public FunctionSeries()
        {
            points = new List<Vector2>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionSeries" /> class using a function <code>f(x)</code>.
        /// </summary>
        /// <param name="f">The function <code>f(x)</code>.</param>
        /// <param name="x0">The start x value.</param>
        /// <param name="x1">The end x value.</param>
        /// <param name="dx">The increment in x.</param>
        /// <param name="title">The title (optional).</param>
        public FunctionSeries(Func<float, float> f, float x0, float x1, float dx, string title = null)
        {
            this.title = title ?? f.Method.Name;
            points = new List<Vector2>();

            for (float x = x0; x <= x1 + (dx * 0.5); x += dx)
            {
                float y = f(x);
                if (!float.IsNormal(y))
                    y = f(x + dx * .1f);
                points.Add(new Vector2(x, y));
            }
        }

        public List<Vector2> points { get; private set; }
        /// <summary>
        /// Coroutine Starter returns a list of Vector2 from function <code>f(x)</code> with linear steps.
        /// </summary>
        /// <param name="f">The function <code>f(x)</code>.</param>
        /// <param name="x0">The start x value.</param>
        /// <param name="x1">The end x value.</param>
        /// <param name="dx">The increment in x.</param>
        /// <param name="title">The title (optional).</param>
        public static List<Vector2> GetLinear(Func<float, float> f, float x0, float x1,float dx)
        {
            List<Vector2> points = new List<Vector2>();

            for (float x = x0; x <= x1 + (dx * 0.5); x += dx)
            {
                float y = f(x);
                if (!float.IsNormal(y))
                    y = f(x + dx * .1f);
                points.Add(new Vector2(x, y));
            }
            return points;
        }
        /// <summary>
        /// Coroutine Starter returns a list of Vector2 from function <code>f(x)</code> with Power of 10 steps.
        /// </summary>
        /// <param name="f">The function <code>f(x)</code>.</param>
        /// <param name="x0">The start x value.</param>
        /// <param name="x1">The end x value.</param>
        /// <param name="dx">The increment in x.</param>
        /// <param name="title">The title (optional).</param>
        public static List<Vector2> GetLog10(Func<float, float> f, float x0, float x1, float dx)
        {
            List<Vector2> points = new List<Vector2>();
            for (float x = x0;Mathf.Pow(10,x) <= (Mathf.Pow(10, x1) + (Mathf.Pow(10,(dx * 0.5f))));x += dx)
            {
                float y = f(Mathf.Pow(10, x));
                if (!float.IsFinite(Mathf.Pow(10, x)))
                    y = f(x + dx * .1f);
                points.Add(new Vector2(Mathf.Pow(10, x), y));
            }
            return points;
        }
    }
}