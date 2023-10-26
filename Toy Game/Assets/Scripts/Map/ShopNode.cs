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
            return itemPool;
        }
    }
}