/*
Clase usada para dibujar primitivas sobre una imagen dada
-Rectangulos
-Ellipses
-Circulos
de una manera rapida
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace vcJOGJ
{
	
	public class Drawing
	{
		public static void DrawLine(BitmapData bData,PointF p1,PointF p2,Color color)
		{
			int startX = (int)p1.X;
			int startY = (int)p1.Y;
			int stopX  =(int) p2.X;
			int stopY  = (int)p2.Y;

			// draw the line
			int dx = stopX - startX;
			int dy = stopY - startY;
			unsafe{
				if ( Math.Abs( dx ) >= Math.Abs( dy ) )
				{
					// the line is more horizontal, we'll plot along the X axis
					float slope = ( dx != 0 ) ? (float) dy / dx : 0;
					int step = ( dx > 0 ) ? 1 : -1;

					// correct dx so last point is included as well
					dx += step;

					
					// just copy color for none transparent colors
					for ( int x = 0; x != dx; x += step )
					{
						int px = startX + x;
						int py = (int) ( (float) startY + ( slope * (float) x ) );

						byte* ptr = (byte*) bData.Scan0.ToPointer( ) + py * bData.Stride + px *3;

						ptr[2] = color.R;
						ptr[1] = color.G;
						ptr[0] = color.B;
					}
					

				}
				else
				{
					// the line is more vertical, we'll plot along the y axis.
					float slope = ( dy != 0 ) ? (float) dx / dy : 0;
					int step = ( dy > 0 ) ? 1 : -1;

					// correct dy so last point is included as well
					dy += step;

					
					for ( int y = 0; y != dy; y += step )
					{
						int px = (int) ( (float) startX + ( slope * (float) y ) );
						int py = startY + y;

						byte* ptr = (byte*)bData.Scan0.ToPointer( ) + py *bData.Stride + px * 3;

						ptr[2] = color.R;
						ptr[1] = color.G;
						ptr[0] = color.B;
					}
					
				}
			}
			
		}
		public static void DrawRectangles(BitmapData bData,Rectangle r,Color color)
		{
			
			int w=bData.Width;
			int h=bData.Height;
			int stride = bData.Stride;
			IntPtr Scan0 = bData.Scan0;
			
			int rectX1 = r.X;
			int rectY1 = r.Y;
			int rectX2 = r.X + r.Width - 1;
			int rectY2 = r.Y + r.Height - 1;

			
			int startX  = Math.Max( 0, rectX1 );
			int stopX   = Math.Min( w - 1, rectX2 );
			int startY  = Math.Max( 0, rectY1 );
			int stopY   = Math.Min( h - 1, rectY2 );

			unsafe
			{
				byte* ptr = (byte*)Scan0.ToPointer( ) + startY * stride + startX * 3;
				byte red    = color.R;
				byte green  = color.G;
				byte blue   = color.B;

				int offset = stride - ( stopX - startX + 1 ) * 3;
				
				for ( int y = startY; y <= stopY; y++ )
				{
					for ( int x = startX; x <= stopX; x++, ptr += 3 )
					{
						ptr[2] = red;
						ptr[1] = green;
						ptr[0] = blue;
					}
					ptr += offset;
				}
				
			}
		}
		public static void DrawCircle(BitmapData bData,int x0,int y0,int radio,Color color)
		{
			paso=bData.Stride;
			int f = 1 - radio;
			int ddF_x = 1;
			int ddF_y = -2 * radio;
			int x = 0;
			int y = radio;
			byte r=color.R;
			byte g=color.G;
			byte b=color.B;
			
			SetPixel(bData.Scan0,x0, y0 + radio,r,g,b);
			SetPixel(bData.Scan0,x0, y0 - radio,r,g,b);
			SetPixel(bData.Scan0,x0 + radio, y0,r,g,b);
			SetPixel(bData.Scan0,x0 - radio, y0,r,g,b);
			
			while(x < y)
			{
				
				if(f >= 0)
				{
					y--;
					ddF_y += 2;
					f += ddF_y;
				}
				x++;
				ddF_x += 2;
				f += ddF_x;
				SetPixel(bData.Scan0,x0 + x, y0 + y,r,g,b);
				SetPixel(bData.Scan0,x0 - x, y0 + y,r,g,b);
				SetPixel(bData.Scan0,x0 + x, y0 - y,r,g,b);
				SetPixel(bData.Scan0,x0 - x, y0 - y,r,g,b);
				SetPixel(bData.Scan0,x0 + y, y0 + x,r,g,b);
				SetPixel(bData.Scan0,x0 - y, y0 + x,r,g,b);
				SetPixel(bData.Scan0,x0 + y, y0 - x,r,g,b);
				SetPixel(bData.Scan0,x0 - y, y0 - x,r,g,b);
			}

		}
		private static int paso;
		private static void SetPixel( IntPtr data,int x, int y, byte r, byte g, byte b)
		{
			
			unsafe
			{
				
				byte* ptr = (byte*) data.ToPointer( ) + y * paso + x * 3;
				
				*(ptr+2) = r;
				*(ptr+1) = g;
				*(ptr+0) = b;
				
			}
			
		}
	}
}


