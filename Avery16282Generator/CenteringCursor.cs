using System;

namespace Avery16282Generator
{
    public class CenteringCursor
    {
        private readonly float _startAnchor;
        private readonly float _endAnchor;
        public CenteringCursor(float startAnchor, float endAnchor)
        {
            _startAnchor = startAnchor;
            _endAnchor = endAnchor;
        }

        public Cursor StartCursor { get; }= new Cursor();
        public Cursor EndCursor { get; } = new Cursor();

        public float GetCurrentStartWithCentering()
        {
            var startOffset = Math.Abs(_startAnchor - StartCursor.GetCurrent());
            var endOffset = Math.Abs(_endAnchor - EndCursor.GetCurrent());
            var maxOffset = Math.Max(startOffset, endOffset);
            return _startAnchor - maxOffset;
        }

        public float GetCurrentEndWithCentering()
        {
            var startOffset = Math.Abs(_startAnchor - StartCursor.GetCurrent());
            var endOffset = Math.Abs(_endAnchor - EndCursor.GetCurrent());
            var maxOffset = Math.Max(startOffset, endOffset);
            return _endAnchor + maxOffset;
        }
    }
}
