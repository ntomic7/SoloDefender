using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleVisuals : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] ResourceManagerVisuals resourceManagerVisuals;

    [SerializeField] List<GameObject> playObjectsUI;

    [SerializeField] PlayerLevelManager playerLevelManager;
    GameObject playerHealthBar;
    GameObject playerManaBar;

    bool damagingPlayer = false;
    bool isMagicAttack;

    [SerializeField] GameObject playerBars;
    [SerializeField] GameObject enemyBars;
    [SerializeField] public List<GameObject> BattleSpellIcons;
    bool IsFortressAttack;

    // BUFF MANAGMENT
    [SerializeField] public List<Image> buffIconListEnemy;
    [SerializeField] public List<Image> buffIconListPlayer;

    [SerializeField] GameObject enemyHealthBar;

    float cameraSize;
    Vector3 enemyPosition;
    Vector3 playerPosition;
    Vector3 cameraPosition;

    [SerializeField] GameObject playerGO;
    GameObject enemyGO;

    StatusBar playerHealthBarScript;
    StatusBar playerManaBarScript;
    StatusBar enemyHealthBarScript;

    RectTransform playerBarsTransform;
    Vector2 playerBarsPosition = new Vector2(541.65f, 300.8487f);
    Vector2 oldPlayerBarsPosition;

    Attributes playerAttributes;
    Attributes enemyAttributes;

    Animator playerAnimator;
    Animator enemyAnimator;

    void Start()
    {
        playerAttributes = playerGO.GetComponent<Attributes>();
        playerAnimator = playerGO.GetComponentInChildren<Animator>();

        playerHealthBar = playerBars.transform.GetChild(0).gameObject;
        playerManaBar = playerBars.transform.GetChild(1).gameObject;

        playerHealthBarScript = playerHealthBar.GetComponent<StatusBar>();
        playerManaBarScript = playerManaBar.GetComponent<StatusBar>();

        playerHealthBarScript.SetMaxValue(playerAttributes.maxHealth);
        playerManaBarScript.SetMaxValue(playerAttributes.maxMana);
        enemyHealthBarScript = enemyHealthBar.GetComponent<StatusBar>();


        cameraSize = mainCamera.orthographicSize;

        // find battle position for the status bars
        playerBarsTransform = playerBars.GetComponent<RectTransform>();
        oldPlayerBarsPosition = playerBarsTransform.localPosition;

    }


    public void StartBattleSetUp()
    {
        ActivateBattleUI(true);
        resourceManagerVisuals.SetIsBattling(true);
        foreach(GameObject go in playObjectsUI) go.SetActive(false);

        enemyAttributes = enemyGO.GetComponent<Attributes>();

        enemyHealthBarScript.SetMaxValue(enemyAttributes.maxHealth);
        enemyHealthBarScript.SetCurrentValue(enemyAttributes.currentHealth);
        playerHealthBarScript.SetCurrentValue(playerAttributes.currentHealth);

        playerAnimator.transform.rotation = Quaternion.Euler(0, 180, 0);

        playerPosition = playerGO.transform.position;
        enemyPosition = enemyGO.transform.position;
        mainCamera.orthographicSize = 1f;

        // move all objects into battle position
        playerBarsTransform.anchoredPosition = playerBarsPosition;

        if (!IsFortressAttack)
        {
            mainCamera.transform.position = new Vector3(playerGO.transform.position.x, playerGO.transform.position.y + 0.1f, mainCamera.transform.position.z);
            cameraPosition = mainCamera.transform.position;
            enemyGO.transform.position = new Vector3(playerGO.transform.position.x - 1f, playerGO.transform.position.y, playerGO.transform.position.z);
            playerGO.transform.position = new Vector3(playerGO.transform.position.x + 1f, playerGO.transform.position.y, playerGO.transform.position.z);
        }
        else
        {
            mainCamera.transform.position = cameraPosition;
            enemyGO.transform.position = new Vector3(playerGO.transform.position.x - 2f, playerGO.transform.position.y, playerGO.transform.position.z);
        }
    }
    public void ActivateBattleUI(bool option)
    {
        for (int i = 0; i < playerLevelManager.GetActiveAttackNum(); i++)
        {
            BattleSpellIcons[i].gameObject.SetActive(option);
        }
        enemyBars.gameObject.SetActive(option);
    }
    public void EndBattleSetUp()
    {
        ActivateBattleUI(false);

        resourceManagerVisuals.SetIsBattling(false);
        ShowAllCooldownVisuals();
        foreach (GameObject go in playObjectsUI) go.SetActive(true);

        // move all objects into starting positions
        if(enemyGO != null)
            enemyGO.transform.position = enemyPosition;
        mainCamera.orthographicSize = cameraSize;


        playerAnimator.transform.rotation = Quaternion.Euler(0, 0, 0);
        playerAnimator.SetTrigger("OnBattleEnd");
        playerGO.transform.position = playerPosition;
        playerBarsTransform.anchoredPosition = oldPlayerBarsPosition;
    }
    public void PlayerAttackAnimation()
    {
        playerAnimator.SetTrigger("OnAttack");
    }
    public void EnemyAttackAnimation(bool isBossBattle)
    {
        //if(!damagingPlayer)
          //  playerAnimator.SetTrigger("OnAttack");
        if (isBossBattle) enemyAnimator.SetTrigger("OnCharge");
        else enemyAnimator.SetTrigger("OnAttack");
    }

    public void StopEnemyMagicAttack()
    {
        enemyAnimator.SetTrigger("OnStopMagic");
    }
    public void BossMagicAttack()
    {
        enemyAnimator.SetTrigger("OnMagic");

    }
    public void DamagePlayer()
    {
        playerAnimator.SetTrigger("OnHurt");
        damagingPlayer = true;
    }

    public void DeathAnimation(GameObject character)
    {
        if (character.GetComponent<PlayerMovement>() != null)
        {
            Debug.Log(isMagicAttack + " IS BOSS BATTLE");

            if (!isMagicAttack) playerAnimator.SetTrigger("OnDeath");
            else StartCoroutine(WaitForDeathTick());
        }
        else
        {
            enemyAnimator.SetTrigger("OnDeath");
        }
    }
    IEnumerator WaitForDeathTick()
    {
        yield return new WaitForSeconds(.7f);

        playerAnimator.SetBool("IsDeath", true);

    }
    public void RoundVisualsBoth(int damageToPlayer, int damageToEnemy)
    {
        playerHealthBarScript.SetValue(damageToPlayer);
        enemyHealthBarScript.SetValue(damageToEnemy);
    }
    public void RoundVisualsPlayer(int playerManaAmount, int playerHealthAmount)
    {
        playerManaBarScript.SetValue(playerManaAmount);
        playerHealthBarScript.SetValue(playerHealthAmount);
    }


    public void AddBuffs(List<SpellObjects> currentBuffList, List<Image> currentIconList)
    {
        foreach (Image icon in currentIconList)
        {
            icon.gameObject.SetActive(false);
        }

        for (int i = 0; i < Mathf.Min(currentBuffList.Count, currentIconList.Count); i++)
        {
            SpellObjects spellObject = currentBuffList[i];
            currentIconList[i].gameObject.SetActive(true);
            currentIconList[i].sprite = spellObject.GetBuffSprite();
        }
    }
    public void RemoveBuffs(List<SpellObjects> currentSpellList, List<Image> currentIconList)
    {
        AddBuffs(currentSpellList, currentIconList);
    }

    public void SkillCooldownVisuals(SpellObjects spellObject, bool IsDeleting)
    {
        int numOfIcons = 6;

        Vector3Int backgroundImageNum = (IsDeleting) ? new Vector3Int(255, 255, 255) : new Vector3Int(107, 105, 105);
        Vector3Int skillImageNum = (IsDeleting) ? new Vector3Int(255, 255, 255) : new Vector3Int(113, 113, 113);

        for(int i = 0; i < numOfIcons; i++)
        {
            if (BattleSpellIcons[i].GetComponent<BattleResourcesManager>().GetResource() != null &&
                BattleSpellIcons[i].GetComponent<BattleResourcesManager>().GetResource() == spellObject)
            {
                Image backgroundImage = BattleSpellIcons[i].GetComponent<Image>();
                Image skillImage = backgroundImage.gameObject.transform.GetChild(0).GetComponent<Image>();
                TextMeshProUGUI coolDownText = backgroundImage.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();    

                backgroundImage.color = new Color32((byte)backgroundImageNum.x, (byte)backgroundImageNum.y, (byte)backgroundImageNum.z, 255);
                skillImage.color = new Color32((byte)skillImageNum.x, (byte)skillImageNum.y, (byte)skillImageNum.z, 255);
                if (IsDeleting) coolDownText.text = "";
                else coolDownText.text = spellObject.currentSkillCooldown.ToString();
            }
        }
    }
    void ShowAllCooldownVisuals()
    {
        int numOfIcons = 6;
        for (int i = 0; i < numOfIcons; i++)
        {
            Image backgroundImage = BattleSpellIcons[i].GetComponent<Image>();
            Image skillImage = backgroundImage.gameObject.transform.GetChild(0).GetComponent<Image>();
            TextMeshProUGUI coolDownText = backgroundImage.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            backgroundImage.color = new Color32(255, 255, 255, 255);
            skillImage.color = new Color32(255, 255, 255, 255);
            coolDownText.text = "";

        }
    }

    public void SetEnemyGO(GameObject enemyGO)
    {
        this.enemyGO = enemyGO;
        enemyAnimator = enemyGO.GetComponentInChildren<Animator>();

    }
    public void SetIsFortressAttack(bool option)
    {
       IsFortressAttack = option;
    }
    public void SetIsMagicAttack(bool option)
    {
        isMagicAttack = option;
    }

}
