using NUnit.Framework;
using System;
using System.Collections.Generic;
using Tree = BinSearchTreeDictionary<int, int>;
using TreePair = System.Collections.Generic.KeyValuePair<int, int>;

public class TestsEmpty
{

    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void TestAdd()
    {
        Tree tree = new Tree();

        Assert.IsFalse(tree.ContainsKey(1));
        tree.Add(1, 10);
        Assert.IsTrue(tree.ContainsKey(1));
        Assert.IsTrue(tree[1] == 10);
    }

    [Test]
    public void TestCount()
    {
        Tree tree = new Tree();
        Assert.IsTrue(tree.Count() == 0);

        tree.Add(1, 1);
        Assert.IsTrue(tree.Count() == 1);

        tree.Add(2, 2);
        Assert.IsTrue(tree.Count() == 2);

        tree.Remove(2);
        Assert.IsTrue(tree.Count() == 1);

        tree.Remove(1);
        Assert.IsTrue(tree.Count() == 0);
    }

    [Test]
    [Description("Удаление из пустого дерева")]
    public void TestDeleteFromEmpty()
    {
        Tree tree = new Tree();
        Assert.IsFalse(tree.Remove(1));
        Assert.IsTrue(tree.Count() == 0);
    }

    [Test]
    [Description("Простое удаление из дерева с одним элементом")]
    public void TestDeleteSingleRoot()
    {
        Tree tree = new Tree();
        tree.Add(1, 1);
        Assert.IsTrue(tree.ContainsKey(1));
        Assert.IsTrue(tree.Remove(1));
        Assert.IsFalse(tree.ContainsKey(1));
    }

    [Test]
    [Description("Удаление нескольких последовательных элементов")]
    public void TestDelete2()
    {
        Tree tree = new Tree();
        tree.Add(2, 2);
        tree.Add(1, 1);
        tree.Add(3, 3);

        Assert.IsTrue(tree.Count() == 3);
        Assert.IsTrue(tree.ContainsKey(1));
        Assert.IsTrue(tree.ContainsKey(2));
        Assert.IsTrue(tree.ContainsKey(3));

        Assert.IsTrue(tree.Remove(2));

        Assert.IsTrue(tree.Count() == 2);
        Assert.IsTrue(tree.ContainsKey(1));
        Assert.IsFalse(tree.ContainsKey(2));
        Assert.IsTrue(tree.ContainsKey(3));
        Assert.IsFalse(tree.Remove(2));

        Assert.IsTrue(tree.Remove(3));

        Assert.IsTrue(tree.Count() == 1);
        Assert.IsTrue(tree.ContainsKey(1));
        Assert.IsFalse(tree.ContainsKey(2));
        Assert.IsFalse(tree.ContainsKey(3));
        Assert.IsFalse(tree.Remove(3));

        Assert.IsTrue(tree.Remove(1));

        Assert.IsTrue(tree.Count() == 0);
        Assert.IsFalse(tree.ContainsKey(1));
        Assert.IsFalse(tree.ContainsKey(2));
        Assert.IsFalse(tree.ContainsKey(3));
        Assert.IsFalse(tree.Remove(1));
    }

    [Test]
    [Description("Удаление случайных элементов")]
    public void TestDeleteRandom()
    {
        Tree tree = new Tree();

        HashSet<int> keysSet = new HashSet<int>();

        Random rnd = new Random();
        int count = (rnd.Next() % 4096) + 8;
        for (int i = 0; i < count; i++)
        {
            keysSet.Add(rnd.Next());
        }

        List<int> keys = new List<int>();
        foreach (int key in keysSet)
        {
            //Assert.Warn("- " + key);
            tree.Add(key, key);
            keys.Add(key);
        }

        while (keys.Count > 0)
        {
            int ind = rnd.Next() % keys.Count;
            int key = keys[ind];

            keys.Remove(key);

            Assert.IsTrue(tree.ContainsKey(key));
            //Assert.Warn(tree.Count().ToString());
            Assert.IsTrue(tree.Remove(key));
            //Assert.Warn(tree.Count().ToString());
            Assert.IsFalse(tree.ContainsKey(key));
        }
    }

    [Description("Удаление нескольких последовательных элементов")]
    // Вытекал из теста выше и является нормализованным вариантом одного из его прогонов, который выдавал ошибку
    public void TestDeleteSet()
    {
        Tree tree = new Tree();

        HashSet<int> keysSet = new HashSet<int>();

        keysSet.Add(9);
        keysSet.Add(7);
        keysSet.Add(14);
        keysSet.Add(13);
        keysSet.Add(10);
        keysSet.Add(11);
        keysSet.Add(12);
        keysSet.Add(15);
        keysSet.Add(5);
        keysSet.Add(8);
        keysSet.Add(3);
        keysSet.Add(0);
        keysSet.Add(6);
        keysSet.Add(2);
        keysSet.Add(1);
        keysSet.Add(4);

        List<int> keys = new List<int>();
        foreach (int key in keysSet)
        {
            tree.Add(key, key);
            keys.Add(key);
        }

        Assert.Warn("Count: " + tree.Count().ToString());
        while (keys.Count > 0)
        {
            int key = keys[0];

            keys.Remove(key);

            Assert.IsTrue(tree.ContainsKey(key));
            Assert.Warn("Key: " + key);
            Assert.IsTrue(tree.Remove(key));
            Assert.Warn("Count: " + tree.Count().ToString());
            Assert.IsFalse(tree.ContainsKey(key));
        }
    }

    [Test]
    public void TestSerialization()
    {
        Tree tree = new Tree();
        tree.Add(1, 2);

        tree.Serialize("test.bin");

        Tree tree2 = Tree.Deserialize("test.bin");
        Assert.IsTrue(tree.ContainsKey(1));
    }

    [Test]
    public void TestRemovePair()
    {
        Tree tree = new Tree();
        tree.Add(1, 2);
        Assert.IsFalse(tree.Remove(new TreePair(1, 1)));
        Assert.IsTrue(tree.Remove(new TreePair(1, 2)));
    }


    [Test]
    public void TestClear()
    {
        Tree tree = new Tree();
        tree.Add(1, 2);
        Assert.IsTrue(tree.Count() > 0);
        tree.Clear();
        Assert.IsTrue(tree.Count() == 0);
    }

    [Test]
    // Тест проверяет что все значения, записанные в словарь, могут быть получены через иттератор и в них ничего лишнего не будет
    public void TestForeach()
    {
        Tree tree = new Tree();

        Random rnd = new Random();
        int count = rnd.Next(10) + 10;

        HashSet<int> keysSet = new HashSet<int>();
        for (int i = 0; i < count; i++)
        {
            keysSet.Add(rnd.Next(100) + 1);
        }

        foreach (int key in keysSet)
        {
            tree.Add(key, key*10);
        }

        int prevkey = 0;
        foreach (TreePair pair in tree)
        {
            Assert.IsTrue(prevkey < pair.Key);
            Assert.IsTrue(keysSet.Remove(pair.Key));
            Assert.IsTrue(pair.Key*10 == pair.Value);
            prevkey = pair.Key;
        }

        Assert.IsTrue(keysSet.Count == 0);
    }

    [Test]
    public void TestWriteReadWithField()
    {
        Tree tree = new Tree();

        tree[1] = 10;
        Assert.IsTrue(tree[1] == 10);

        tree[1] = -10;
        Assert.IsTrue(tree[1] == -10);

        tree[2] = 20;
        Assert.IsTrue(tree[1] == -10);
        Assert.IsTrue(tree[2] == 20);
    }

    [Test]
    public void TestTryGet()
    {
        Tree tree = new Tree();
        tree.Add(1, 2);
        int val = 0;
        Assert.IsFalse(tree.TryGetValue(2, out val));
        Assert.IsTrue(val == default);
        Assert.IsTrue(tree.TryGetValue(1, out val));
        Assert.IsTrue(val == 2);
    }

}
