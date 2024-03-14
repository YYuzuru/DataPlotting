using UnityEngine;
namespace GraphPlotter.Interface
{
    public interface IColorable
    {
        void SetColor(params Color[] color);
        Color[] GetColor();
    }
}