using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
class BinSearchTreeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{

    [Serializable]
    public class TreeNode
    {

        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public TreeNode Parent { get; set; } = null;
        public TreeNode Left { get; set; } = null;
        public TreeNode Right { get; set; } = null;

        public TreeNode(TKey key, TValue value, TreeNode parent = null, TreeNode left = null, TreeNode right = null)
        {
            this.Key = key;
            this.Value = value;
            this.Parent = parent;
            this.Left = left;
            this.Right = right;
        }

        public void SetLeftNode(TreeNode node)
        {
            if (this.Left != null)
            {
                this.Left.Parent = null;
            }

            if (node != null)
            {
                node.Parent = this;
            }

            this.Left = node;
        }

        public void SetParent(TreeNode node)
        {
            int thisHash = this.Key.GetHashCode();

            if (this.Parent != null)
            {
                if (thisHash < this.Parent.Key.GetHashCode())
                {
                    this.Parent.Left = null;
                }
                else
                {
                    this.Parent.Right = null;
                }
            }

            this.Parent = node;

            if (this.Parent != null)
            {
                if (thisHash < node.Key.GetHashCode())
                {
                    this.Parent.Left = this;
                }
                else
                {
                    this.Parent.Right = this;
                }
            }

        }

    }

    public class TreeNodesEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {

        private readonly TreeNode[] treeNodes;
        private int pos = -1;

        public TreeNodesEnumerator(List<TreeNode> treeNodes)
        {
            this.treeNodes = treeNodes.ToArray();
            this.pos = -1;
        }

        public KeyValuePair<TKey, TValue> Current
        {
            get
            {
                if (this.pos == -1)
                    return new KeyValuePair<TKey, TValue>(default, default);

                TreeNode node = this.treeNodes[this.pos];
                return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        public void Dispose()
        {
            return;
        }

        public bool MoveNext()
        {
            this.pos++;
            return (this.pos < this.treeNodes.Length);
        }

        public void Reset()
        {
            this.pos = -1;
        }
    }

    private TreeNode Root = null;

    private int Count(TreeNode node)
    {
        if (node == null)
        {
            return 0;
        }

        return (this.Count(node.Left) + this.Count(node.Right)) + 1;
    }

    private List<TreeNode> List(TreeNode node, List<TreeNode> List)
    {
        if (node == null)
        {
            return List;
        }

        if (node.Left != null)
        {
            this.List(node.Left, List);
        }

        List.Add(node);

        if (node.Right != null)
        {
            this.List(node.Right, List);
        }

        return List;
    }

    private List<TreeNode> List()
    {
        List<TreeNode> list = new List<TreeNode>();
        return this.List(this.Root, list);
    }

    public TValue this[TKey key]
    {
        get
        {
            TreeNode node = this.Find(key);
            if (node == null)
                return default;
            else
                return node.Value;
        }
        set
        {
            TreeNode node = this.Find(key);
            if (node != null)
                node.Value = value;
            else
                this.Add(key, value);
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            ICollection<TreeNode> treeNodes = this.List();
            ICollection<TKey> treeKeys = new List<TKey>();

            foreach (TreeNode node in treeNodes)
            {
                treeKeys.Add(node.Key);
            }

            return treeKeys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            ICollection<TreeNode> treeNodes = this.List();
            ICollection<TValue> treeValues = new List<TValue>();

            foreach (TreeNode node in treeNodes)
            {
                treeValues.Add(node.Value);
            }

            return treeValues;
        }
    }

    public int Count()
    {
        return this.Count(Root);
    }

    public bool IsReadOnly
    {
        get
        {
            return true;
        }
    }

    int ICollection<KeyValuePair<TKey, TValue>>.Count
    {
        get
        {
            return this.Count();
        }
    }

    private void Add(TreeNode node, TreeNode current)
    {
        if (current.Key.GetHashCode() == node.Key.GetHashCode())
        {
            node.Parent = current.Parent;
            node.Left = current.Left;
            node.Right = current.Right;

            if (node.Left != null)
                node.Left.Parent = node;
            if (node.Right != null)
                node.Right.Parent = node;

            if (current == this.Root)
            {
                this.Root = node;
            }
            else
            {
                if (current.Parent.Left == current)
                {
                    current.Parent.Left = node;
                }
                else
                {
                    current.Parent.Right = node;
                }
            }

            return;
        }

        if (node.Key.GetHashCode() < current.Key.GetHashCode())
        {
            if (current.Left == null)
            {
                node.Parent = current;
                current.Left = node;
            }
            else
            {
                this.Add(node, current.Left);
            }
        }
        else
        {
            if (current.Right == null)
            {
                node.Parent = current;
                current.Right = node;
            }
            else
            {
                this.Add(node, current.Right);
            }
        }
    }

    private void Add(TreeNode node)
    {
        if (this.Root == null)
        {
            this.Root = node;
            return;
        }

        this.Add(node, this.Root);
    }

    public void Add(TKey key, TValue value)
    {
        this.Add(new TreeNode(key, value));
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        this.Add(item.Key, item.Value);
    }

    public void Clear()
    {
        this.Root = null;
    }

    private TreeNode Find(TKey key, TreeNode node)
    {
        if (node == null)
            return null;

        int keyHash = key.GetHashCode();
        int nodeHash = node.Key.GetHashCode();

        if (keyHash == nodeHash)
            return node;

        if (keyHash < nodeHash)
            return this.Find(key, node.Left);

        return this.Find(key, node.Right);
    }

    private TreeNode Find(TKey key)
    {
        return this.Find(key, this.Root);
    }

    public bool ContainsKey(TKey key)
    {
        return (this.Find(key) != null);
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        TreeNode node = this.Find(item.Key);
        if (node == null)
            return false;
        return node.Value.GetHashCode() == item.Value.GetHashCode();
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return new TreeNodesEnumerator(this.List());
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    private TreeNode Leftmost(TreeNode node)
    {
        if (node == null)
            return null;

        while (node.Left != null)
            node = node.Left;

        return node;
    }

    private bool Delete(TKey key, TreeNode node)
    {
        if (node == null)
            return false;

        int keyHash = key.GetHashCode();
        int nodeHash = node.Key.GetHashCode();

        if (keyHash < nodeHash)
            return this.Delete(key, node.Left);

        if (keyHash > nodeHash)
            return this.Delete(key, node.Right);

        if (node.Left == null && node.Right == null)
        {
            node.SetParent(null);

            if (node == this.Root)
                this.Root = null;
        }
        else
        {
            if (node.Left == null || node.Right == null)
            {
                if (node.Left == null)
                {
                    TreeNode r = node.Right;
                    r.SetParent(node.Parent);

                    if (node == this.Root)
                        this.Root = r;
                }
                else
                {
                    TreeNode l = node.Left;
                    l.SetParent(node.Parent);

                    if (node == this.Root)
                        this.Root = l;
                }
            }
            else
            {
                TreeNode rl = node.Right.Left;

                if (rl == null)
                {
                    TreeNode r = node.Right;

                    r.SetLeftNode(node.Left);
                    r.SetParent(node.Parent);

                    if (node == this.Root)
                        this.Root = r;
                }
                else
                {
                    TreeNode tlm = this.Leftmost(node.Right);
                    tlm.SetLeftNode(node.Left);
                    if (tlm.Right != null)
                        tlm.Parent.SetLeftNode(tlm.Right);
                    tlm.SetParent(node.Parent);

                    node.Right.Parent = tlm;
                    tlm.Right = node.Right;

                    if (node == this.Root)
                        this.Root = tlm;
                }
            }
        }

        return true;
    }

    private bool Delete(TKey key)
    {
        return this.Delete(key, this.Root);
    }

    public bool Remove(TKey key)
    {
        return this.Delete(key);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        TreeNode node = this.Find(item.Key);
        if (node == null)
            return false;
        if (node.Value.GetHashCode() != item.Value.GetHashCode())
            return false;
        return this.Remove(item.Key);
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        TreeNode node = this.Find(key);

        if (node == null)
        {
            value = default;
            return false;
        }
        else
        {
            value = node.Value;
            return true;
        }
    }

    public String Serialize()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            formatter.Serialize(ms, this);
            return Convert.ToBase64String(ms.ToArray());
        }
    }

    static public BinSearchTreeDictionary<TKey, TValue> Deserialize(String data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(data)))
        {
            BinSearchTreeDictionary<TKey, TValue> tree = (BinSearchTreeDictionary<TKey, TValue>)formatter.Deserialize(ms);
            return tree;
        }
    }

}
