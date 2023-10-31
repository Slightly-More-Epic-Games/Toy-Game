using System.Collections;
using System.Collections.Generic;
using HoverUI;
using Items;
using Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private HoverableUI buttonPrefab;

    [SerializeField] private Transform playerParent;
    [SerializeField] private Transform shopParent;

    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private Button continueButton;

    private List<ShopItem> shopItems = new List<ShopItem>();
    private List<ShopItem> playerItems = new List<ShopItem>();

    private void Start() {
        ShopNode node = (ShopNode)Manager.instance.currentNode;

        foreach (ItemSlot itemSlot in Game.instance.player.items) {
            CreateShopItem(itemSlot, true);
        }

        foreach (Item item in node.GetItems()) {
            item.ui.SetItem(item);
            CreateShopItem(new ItemSlot(item), false);
        }

        ChangeHealth(0);
    }

    private void CreateShopItem(ItemSlot itemSlot, bool isPlayer) {
        HoverableUI button = Instantiate(buttonPrefab, isPlayer ? playerParent : shopParent);
        ItemUI itemUI = itemSlot.GetItemUI();
        button.SetInfo(itemUI);
        button.image.sprite = itemUI.icon;
        button.FlipOffset();
        ShopItem shopItem = new ShopItem(button, itemSlot, isPlayer);
        button.onClick.AddListener(delegate {SwitchItem(shopItem);});
        if (isPlayer) {
            playerItems.Add(shopItem);
        } else {
            shopItems.Add(shopItem);
        }
    }

    public void SwitchItem(ShopItem shopItem) {
        if (playerItems.Contains(shopItem)) {
            shopItem.hoverableUI.transform.SetParent(shopParent);
            playerItems.Remove(shopItem);
            shopItems.Add(shopItem);
            ChangeHealth(10);
        } else {
            shopItem.hoverableUI.transform.SetParent(playerParent);
            playerItems.Add(shopItem);
            shopItems.Remove(shopItem);
            ChangeHealth(-10);
        }
    }

    private void ChangeHealth(int delta) {
        Creature playerCreature = Game.instance.player;
        playerCreature.health += delta;

        string text = "Health: "+Mathf.Clamp(playerCreature.health, 0, playerCreature.maxHealth);
        if (playerCreature.health < 0) {
            text += " ("+playerCreature.health+")";
        } else if (playerCreature.health > playerCreature.maxHealth) {
            text += " (+"+(playerCreature.health-playerCreature.maxHealth)+")";
        }
        health.text = text;

        continueButton.interactable = playerCreature.health > 0;
    }

    public void Continue() {
        Game.instance.player.items.Clear();
        foreach (ShopItem shopItem in playerItems) {
            ItemSlot itemSlot = shopItem.newItem ? shopItem.itemSlot.Copy() : shopItem.itemSlot;
            Game.instance.player.items.Add(itemSlot);
        }
        Game.instance.player.health = Mathf.Clamp(Game.instance.player.health, 0, Game.instance.player.maxHealth);

        Game.instance.LoadGameScene(Game.GameScene.Map);
    }

    public class ShopItem {
        public ShopItem(HoverableUI hoverableUI, ItemSlot itemSlot, bool playerOwned) {
            this.hoverableUI = hoverableUI;
            this.itemSlot = itemSlot;
            this.newItem = !playerOwned;
        }

        public HoverableUI hoverableUI;
        public ItemSlot itemSlot;
        public bool newItem;
    }
}
