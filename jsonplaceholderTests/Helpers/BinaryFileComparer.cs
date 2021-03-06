﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Helpers
{
    public class BinaryFileComparer : IEqualityComparer<string>
    {
        public bool Equals(string file1, string file2)
        {
            byte[] file1bytes = File.ReadAllBytes(file1);
            byte[] file2bytes = File.ReadAllBytes(file2);

            if (file1bytes.Length == file2bytes.Length)
            {
                for (int i = 0; i < file1bytes.Length; i++)
                {
                    if (!(file1bytes[i] == file2bytes[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public int GetHashCode(string obj)
        {
            throw new NotImplementedException();
        }
    }
}
