using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using GXPEngine.OpenGL;

namespace GXPEngine.Core
{
    public class Texture2D
    {
        private const int UNDEFINED_GLTEXTURE = 0;
        private static readonly Hashtable LoadCache = new Hashtable();
        private static Texture2D lastBound;

        private Bitmap _bitmap;
        private string _filename;
        private int[] _glTexture;

        //------------------------------------------------------------------------------------------------------------------------
        //														Texture2D()
        //------------------------------------------------------------------------------------------------------------------------
        public Texture2D(int width, int height)
        {
            if (width == 0) if (height == 0) return;
            SetBitmap(new Bitmap(width, height));
        }

        public Texture2D(string filename)
        {
            Load(filename);
        }

        public Texture2D(Bitmap bitmap)
        {
            SetBitmap(bitmap);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetInstance()
        //------------------------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------------------------
        //														bitmap
        //------------------------------------------------------------------------------------------------------------------------
        public Bitmap bitmap
        {
            get { return _bitmap; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														filename
        //------------------------------------------------------------------------------------------------------------------------
        public string filename
        {
            get { return _filename; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														width
        //------------------------------------------------------------------------------------------------------------------------
        public int width
        {
            get { return _bitmap.Width; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														height
        //------------------------------------------------------------------------------------------------------------------------
        public int height
        {
            get { return _bitmap.Height; }
        }

        public static Texture2D GetInstance(string filename)
        {
            var tex2d = LoadCache[filename] as Texture2D;
            if (tex2d == null)
            {
                tex2d = new Texture2D(filename);
                LoadCache[filename] = tex2d;
            }
            return tex2d;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Bind()
        //------------------------------------------------------------------------------------------------------------------------
        public void Bind()
        {
            if (lastBound == this) return;
            lastBound = this;
            GL.BindTexture(GL.TEXTURE_2D, _glTexture[0]);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Unbind()
        //------------------------------------------------------------------------------------------------------------------------
        public void Unbind()
        {
            //GL.BindTexture (GL.TEXTURE_2D, 0);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Load()
        //------------------------------------------------------------------------------------------------------------------------
        private void Load(string filename)
        {
            _filename = filename;
            Bitmap bitmap;
            try
            {
                bitmap = new Bitmap(filename);
            }
            catch
            {
                throw new Exception("Image " + filename + " cannot be found.");
            }
            SetBitmap(bitmap);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SetBitmap()
        //------------------------------------------------------------------------------------------------------------------------
        private void SetBitmap(Bitmap bitmap)
        {
            _bitmap = bitmap;
            CreateGLTexture();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														CreateGLTexture()
        //------------------------------------------------------------------------------------------------------------------------
        private void CreateGLTexture()
        {
            if (_glTexture != null)
                if (_glTexture.Length > 0)
                    if (_glTexture[0] != UNDEFINED_GLTEXTURE)
                        destroyGLTexture();

            _glTexture = new int[1];
            if (_bitmap == null)
                _bitmap = new Bitmap(64, 64);

            GL.GenTextures(1, _glTexture);

            GL.BindTexture(GL.TEXTURE_2D, _glTexture[0]);
            GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR); //GL.NEAREST);
            GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, GL.LINEAR);
            GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, GL.GL_CLAMP_TO_EDGE_EXT);
            GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, GL.GL_CLAMP_TO_EDGE_EXT);

            UpdateGLTexture();
            GL.BindTexture(GL.TEXTURE_2D, 0);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														UpdateGLTexture()
        //------------------------------------------------------------------------------------------------------------------------
        public void UpdateGLTexture()
        {
            BitmapData data = _bitmap.LockBits(new System.Drawing.Rectangle(0, 0, _bitmap.Width, _bitmap.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            GL.BindTexture(GL.TEXTURE_2D, _glTexture[0]);
            GL.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, _bitmap.Width, _bitmap.Height, 0,
                GL.BGRA, GL.UNSIGNED_BYTE, data.Scan0);

            _bitmap.UnlockBits(data);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														destroyGLTexture()
        //------------------------------------------------------------------------------------------------------------------------
        private void destroyGLTexture()
        {
            GL.DeleteTextures(1, _glTexture);
            _glTexture[0] = UNDEFINED_GLTEXTURE;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Clone()
        //------------------------------------------------------------------------------------------------------------------------
        public Texture2D Clone(bool deepCopy = false)
        {
            Bitmap bitmap;
            if (deepCopy)
            {
                bitmap = _bitmap.Clone() as Bitmap;
            }
            else
            {
                bitmap = _bitmap;
            }
            var newTexture = new Texture2D(0, 0);
            newTexture.SetBitmap(bitmap);
            return newTexture;
        }
    }
}