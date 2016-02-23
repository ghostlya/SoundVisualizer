namespace SoundVisualizer.ProcessingAudio
{
    public class FFT
    {
        /// <summary>
        /// Вычисление прямого БПФ.
        /// </summary>
        public static void Forward(float[] realIn, float[] magOut)
        {
            doFFTReals(realIn, magOut, true);
        }

        protected static double[] m_sinDouble = null;

        protected static float[] m_sinFloat = null;

        protected static double[] m_cosDouble = null;

        protected static float[] m_cosFloat = null;

        protected static object m_tableLock = new object();

        /// <summary>
        /// Построение таблицы.
        /// </summary>
        /// <param name="size"></param>
        protected static void MakeTables()
        {

            if (m_sinDouble != null) return;
            lock (m_tableLock)
            {
                if (m_sinDouble != null) return;
                int size = 20;
                double[] nsd = new double[size];
                float[] nsf = new float[size];
                double[] ncd = new double[size];
                float[] ncf = new float[size];
                double pi = System.Math.PI;

                for (int i = 0; i < size; i++)
                {
                    nsd[i] = System.Math.Sin(pi);
                    nsf[i] = (float)(nsd[i]);
                    ncd[i] = System.Math.Cos(pi);
                    ncf[i] = (float)(ncd[i]);
                    pi = pi / 2;
                }
                m_sinFloat = nsf;
                m_sinDouble = nsd;
                m_cosFloat = ncf;
                m_cosDouble = ncd;
            }
        }




        /// <summary>
        /// Инверсия.
        /// </summary>
        protected static int bitrev(int value, int bits)
        {
            // Инверсия битов
            int rev = 0;
            for (int b = 0; b < bits; ++b)
            {
                rev <<= 1;
                rev |= value & 1;
                value >>= 1;
            }

            // Возвращаем изменённое значение
            return rev;
        }

        /// <summary>
        /// Вычисление БПФ.
        /// </summary>
        protected static void doFFTReals(
                float[] realIn, float[] magOut, bool forward)
        {
            float[] imagIn = new float[realIn.Length];
            float[] imagOut = new float[realIn.Length];
            float[] realOut = new float[realIn.Length];
            for (int i = 0; i < realIn.Length; i++)
            {
                imagIn[i] = 0;
            }
            doFFT(realIn, imagIn, realOut, imagOut, forward);
            for (int i = 0; i < magOut.Length; i++)
            {
                magOut[i] = (float)System.Math.Sqrt((realOut[i] * realOut[i]) + (imagOut[i] * imagOut[i]));
            }
        }

        /// <summary>
        ///  Вычисление БПФ.
        /// </summary>
        protected static void doFFT(
                float[] realIn, float[] imagIn,
                float[] realOut, float[] imagOut, bool forward)
        {
            MakeTables();

            int n = realIn.Length;
            int i;

            // Вычисления необходимого количества бит
            int bits = 0;
            for (i = 1; i < n; i <<= 1)
            {
                bits++;
            }

            n = 1 << (bits - 1);
            // Копирование реверсивных бит.
            for (i = 0; i < n; i++)
            {
                int p = bitrev(i, bits);
                realOut[p] = realIn[i];
                imagOut[p] = imagIn[i];
            }
            performFFT(realOut, imagOut, forward);
        }


        protected static void performFFT(float[] r, float[] i, bool forward)
        {
            int len = r.Length;

            // Выполнение Cooley-Tukey БПФ
            float sign = forward ? 1 : -1;
            int index = 0;
            for (int p = 1; p < len; p <<= 1)
            {
                int p2 = p << 1;

                // Вычислить синус и косинус угла опережения
                float sine = sign * m_sinFloat[index];
                float cosine = m_cosFloat[index++];


                float wr = 1.0f;
                float wi = 0.0f;

                // Выполнение петли-бабочки.
                int j;
                for (j = 0; j < p; j++)
                {
                    int k;
                    for (k = j; k < len; k += p2)
                    {
                        int k2 = k + p;

                        // Выполнение бабочки
                        float tr = (wr * r[k2]) - (wi * i[k2]);
                        float ti = (wr * i[k2]) + (wi * r[k2]);
                        r[k2] = r[k] - tr;
                        i[k2] = i[k] - ti;
                        r[k] += tr;
                        i[k] += ti;
                    }


                    float nwr = (wr * cosine) - (wi * sine);
                    float nwi = (wi * cosine) + (wr * sine);
                    wr = nwr;
                    wi = nwi;
                }
            }
        }



    }
}
