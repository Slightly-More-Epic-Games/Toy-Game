using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Map {
    [CreateAssetMenu(fileName = "ShopNode", menuName = "Toy Game/Map/ShopNode", order = 0)]
    public class ShopNode : Node
    {
        public List<Item> itemPool;

        public int sellCount;

        public List<Item> GetItems() {
            List<Item> itemTemp = new List<Item>(itemPool);
            List<Item> items = new List<Item>(sellCount);

            for (int i = 0; i < sellCount; i++) {
                int index = Random.Range(0, itemTemp.Count);
                items.Add(itemTemp[index]);
                itemTemp.RemoveAt(index);
            }

            return items;
        }
    }
}