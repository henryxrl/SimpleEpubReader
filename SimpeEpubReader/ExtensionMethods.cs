using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpeEpubReader
{
	public static class ExtensionMethods
	{
		public static Boolean hasChild(this TreeNode n)
		{
			if (n.GetNodeCount(true) == 0)
				return false;
			else
				return true;
		}

		public static TreeNode RealNextNode(this TreeNode n)
		{
			return findNextNode(n, true);
		}

		private static TreeNode findNextNode(TreeNode n, Boolean notVisited)
		{
			if (notVisited)
			{
				if (n.hasChild())
				{
					return n.FirstNode;
				}
				else
				{
					return hasNoChildNext(n);
				}
			}
			else
				return hasNoChildNext(n);
		}

		private static TreeNode hasNoChildNext(TreeNode n)
		{
			if (n.NextNode != null)
			{
				return n.NextNode;
			}
			else
			{
				if (n.Parent != null)
				{
					if (n.Parent.NextNode != null)
					{
						return n.Parent.NextNode;
					}
					else
					{
						return findNextNode(n.Parent, false);
					}
				}
				else
				{
					return null;
				}
			}
		}

		public static TreeNode RealPrevNode(this TreeNode n)
		{
			return findPrevNode(n, true);
		}

		private static TreeNode findPrevNode(TreeNode n, Boolean notVisited)
		{
			if (notVisited)
			{
				if (n.PrevNode != null)
				{
					return hasPrev(n.PrevNode);
				}
				else
				{
					if (n.Parent != null)
					{
						return n.Parent;
					}
					else
					{
						return null;
					}
				}
			}
			else
			{
				return hasPrev(n);
			}
		}

		private static TreeNode hasPrev(TreeNode n)
		{
			if (n.hasChild())
			{
				return findPrevNode(n.LastNode, false);
			}
			else
			{
				return n;
			}
		}
	}
}
