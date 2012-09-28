using System;
using Microsoft.SPOT;

namespace Asec.Robotics
{
	public class StringBuilder
	{
		char[] buffer = null;
		int size = 0;

		public StringBuilder(int size = 20)
		{
			buffer = new char[size];
		}

		public void Append(char c)
		{
			if (size + 1 >= buffer.Length)
			{
				char[] buffer2 = new char[buffer.Length * 4 / 3];
				Array.Copy(buffer, buffer2, buffer.Length);
				buffer = buffer2;
				Debug.GC(true);
			}
			buffer[size++] = c;
			buffer[size] = '\0';
		}

		public void Append(String s)
		{
			foreach (char c in s)
				Append(c);
		}

		public void Clear()
		{
			size = 0;
			buffer[0] = '\0';
		}
		public override String ToString()
		{
			return new String(buffer);
		}

		public void AppendFormat(String format, params object[] args)
		{
			int i = 0;
			foreach (object arg in args)
				format = substitute(format, "{" + (i++) + "}", arg);
			Clear();
			Append(format);
		}

		String substitute(string format, String key, object val)
		{
			int i;
			if (val == null) val = string.Empty;
			if ((i = format.IndexOf(key)) != -1)
				format = format.Substring(0, i) + val.ToString() + format.Substring(i + key.Length);
			Debug.GC(true);
			return format;
		}
	}
}
