using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections;

namespace DP.Web.UI
{
    public class TreeViewHelper
    {
        /// <summary>
        /// 递归设置 TreeView 当前节点的所有子节点 Checked 状态 
        /// Checks the child nodes.
        /// </summary>
        /// <param name="pnode">The pnode.</param>
        /// <param name="bchecked">if set to <c>true</c> [bchecked].</param>
        public static void SetChildNodesChecked(TreeNode pnode, bool bchecked)
        {
            pnode.Checked = bchecked;
            foreach (TreeNode node in pnode.ChildNodes)
            {
                SetChildNodesChecked(node, bchecked);
            }
        }

        /// <summary>
        /// 递归获取 TreeView 当前节点及所有子节点 Checked 状态为 True 的Value和Text的列表
        /// Gets the tree view checked value.
        /// </summary>
        /// <param name="pnode">The pnode.</param>
        /// <param name="dict">The dict.</param>
        public static void GetTreeViewCheckedValue(TreeNode pnode, ref Dictionary<string, string> dict)
        {
            GetTreeViewCheckedValue(pnode, ref dict, true);
        }

        /// <summary>
        /// 递归获取 TreeView 当前节点及所有子节点 Checked 状态为 True 的Value和Text的列表
        /// Gets the tree view checked value.
        /// </summary>
        /// <param name="pnode">The pnode.</param>
        /// <param name="ht">The ht.</param>
        public static void GetTreeViewCheckedValue(TreeNode pnode, ref Hashtable ht)
        {
            GetTreeViewCheckedValue(pnode, ref ht, true);
        }

        /// <summary>
        /// 递归获取 TreeView 当前节点及所有子节点 Checked 状态为 True 的Value和Text的列表
        /// Gets the tree view checked value.
        /// </summary>
        /// <param name="pnode">The pnode.</param>
        /// <param name="dict">The dict.</param>
        /// <param name="bContinue">if set to <c>true</c> [b continue].如果父节点 Checked 为 Flase 是否继续。</param>
        public static void GetTreeViewCheckedValue(TreeNode pnode, ref Dictionary<string, string> dict, bool bContinue)
        {
            if (pnode.Checked)
            {
                dict.Add(pnode.Value, pnode.Text);
            }
            foreach (TreeNode node in pnode.ChildNodes)
            {
                GetTreeViewCheckedValue(node, ref dict, bContinue);
            }
        }

        /// <summary>
        /// 递归获取 TreeView 当前节点及所有子节点 Checked 状态为 True 的Value和Text的列表
        /// Gets the tree view checked value.
        /// </summary>
        /// <param name="pnode">The pnode.</param>
        /// <param name="ht">The ht.</param>
        /// <param name="bContinue">if set to <c>true</c> [b continue]..如果父节点 Checked 为 Flase 是否继续。</param>
        public static void GetTreeViewCheckedValue(TreeNode pnode, ref Hashtable ht, bool bContinue)
        {
            if (pnode.Checked)
            {
                ht.Add(pnode.Value, pnode.Text);
            }
            foreach (TreeNode node in pnode.ChildNodes)
            {
                GetTreeViewCheckedValue(node, ref ht, bContinue);
            }
        }


    }
}
