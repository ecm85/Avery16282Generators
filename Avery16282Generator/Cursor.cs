﻿namespace Avery16282Generator
{
    public class Cursor
    {
        private float Current { get; set; }

        public void AdvanceCursor(float increment)
        {
            Current += increment;
        }

        public float GetCurrent()
        {
            return Current;
        }
    }
}
