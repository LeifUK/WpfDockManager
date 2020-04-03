using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace BetterWpfControls.Panels
{
	public class ResizablePanel : Panel
	{
		#region Methods

		protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
		{
			var items = InternalChildren.OfType<UIElement>().ToList();

			foreach (var item in items)
			{
				item.Measure(availableSize);
			}

			var desiredSize = 
				items.Count == 0
				? new Size(0, 0)
				: new Size(items.Sum(c => c.DesiredSize.Width), Math.Min(availableSize.Height, items.Max(c => c.DesiredSize.Height)));

			if (desiredSize.Width <= availableSize.Width)
			{
				return desiredSize;
			}

			var sizes = GetSizes(availableSize.Width, items);

			foreach (var item in items)
			{
				item.Measure(new Size(sizes[item], desiredSize.Height));
			}

			return new Size(availableSize.Width, desiredSize.Height);
		}

		protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
		{
			var items = InternalChildren.OfType<UIElement>().ToList();
			if (items.Count == 0)
			{
				return base.ArrangeOverride(finalSize);
			}

			var totalWidth = items.Sum(i => i.DesiredSize.Width);
			if (totalWidth <= finalSize.Width)
			{
				var offset = 0.0;
				foreach (var item in items)
				{
					item.Arrange(new Rect(new Point(offset, 0), item.DesiredSize));
					offset += item.DesiredSize.Width;
				}
			}
			else
			{
				var sizes = GetSizes(finalSize.Width, items);

				var offset = 0.0;
				foreach (var item in items)
				{
					item.Arrange(new Rect(new Point(offset, 0), new Size(sizes[item], finalSize.Height)));
					offset += sizes[item];
				}
			}

			return base.ArrangeOverride(finalSize);
		}

		private static Dictionary<UIElement, double> GetSizes(double availableWidth, List<UIElement> items)
		{
			var totalWidth = items.Sum(i => i.DesiredSize.Width);
			var fatGuys = items.OrderByDescending(i => i.DesiredSize.Width).ToList();
			var sizes = fatGuys.ToDictionary(t => t, t => t.DesiredSize.Width);

			// Trim fat from items
			for (int i = 0; i < fatGuys.Count - 1; i++)
			{
				var fatDelta = sizes[fatGuys[i]] - sizes[fatGuys[i + 1]];
				var fatCum = fatDelta * (i + 1);
				if (totalWidth - fatCum <= availableWidth)
				{
					var fatCut = (totalWidth - availableWidth) / (i + 1);
					for (int j = 0; j <= i; j++)
					{
						sizes[fatGuys[j]] -= fatCut;
					}

					totalWidth = availableWidth;
					break;
				}
				else
				{
					for (int j = 0; j <= i; j++)
					{
						sizes[fatGuys[j]] -= fatDelta;
					}
					totalWidth -= fatCum;
				}
			}

			// Now that we squeezed exessive fat from all items, it's time to equally shrink them
			if (totalWidth > availableWidth)
			{
				var sizeDelta = (totalWidth - availableWidth) / items.Count;
				foreach (var key in sizes.Keys.ToList())
				{
					sizes[key] -= sizeDelta;
				}
			}
			return sizes;
		}

		#endregion Methods
	}
}