namespace GXPEngine
{
    internal class ControlSet
    {
        public int jump;
        public int left;
        public int right;

        public ControlSet(int set)
        {
            if (set == 0)
            {
                left = Key.A;
                right = Key.D;
                jump = Key.SPACE;
            }
            else if (set == 1)
            {
                left = Key.LEFT;
                right = Key.RIGHT;
                jump = Key.UP;
            }
        }
    }
}