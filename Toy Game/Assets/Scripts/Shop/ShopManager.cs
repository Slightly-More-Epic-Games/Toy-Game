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
        //get current node
        ShopNode node = (ShopNode)Manager.instance.currentNode;

        //create player items
        foreach (ItemSlot itemSlot in Game.instance.player.items) {
            CreateShopItem(itemSlot, true);
        }

        //create shop items
        foreach (Item item in node.GetItems()) {
            item.ui.SetItem(item);
            CreateShopItem(new ItemSlot(item), false);
        }

        ChangeHealth(0);
    }

    private void CreateShopItem(ItemSlot itemSlot, bool isPlayer) {
        //create the button for the item
        HoverableUI button = Instantiate(buttonPrefab, isPlayer ? playerParent : shopParent);
        ItemUI itemUI = itemSlot.GetItemUI();
        //set up the hover info
        button.SetInfo(itemUI);
        button.image.sprite = itemUI.icon;
        button.FlipOffset();
        //make ShopItem reference, which keeps track of what the item is and what its corresponding buttton is
        ShopItem shopItem = new ShopItem(button, itemSlot, isPlayer);
        button.onClick.AddListener(delegate {SwitchItem(shopItem);});
        //add to correct item list
        if (isPlayer) {
            playerItems.Add(shopItem);
        } else {
            shopItems.Add(shopItem);
        }
    }

    public void SwitchItem(ShopItem shopItem) {
        //switch the list of the clicked item and update health
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
        //update health
        Creature playerCreature = Game.instance.player;
        playerCreature.health += delta;

        //update health text, this is limited between 0 and 25, but the players actual health isnt, since otherwise selling an item while on high health then buying a new one would make you lose health overall
        string text = "Health: "+Mathf.Clamp(playerCreature.health, 0, playerCreature.maxHealth);
        //any extra health is indicated in brackets
        if (playerCreature.health < 0) {
            text += " ("+playerCreature.health+")";
        } else if (playerCreature.health > playerCreature.maxHealth) {
            text += " (+"+(playerCreature.health-playerCreature.maxHealth)+")";
        }
        health.text = text;

        //dont let the player continue if they over spent
        continueButton.interactable = playerCreature.health > 0;
    }

    public void Continue() {
        //clear players inventory then re fill it with whatever items are in the playerItems list
        Game.instance.player.items.Clear();
        foreach (ShopItem shopItem in playerItems) {
            //shop items arent properly instanced until this point, but player items already are
            ItemSlot itemSlot = shopItem.newItem ? shopItem.itemSlot.Copy() : shopItem.itemSlot;
            Game.instance.player.items.Add(itemSlot);
        }
        //limit players health, since now the purchases have been confirmed
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
