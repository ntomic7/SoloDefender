using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    bool IsBattling;
    bool attacking;

    public Action<bool> OnBattleStart;
    Coroutine currentCoroutine;

    public bool canUseResource;
    public bool IsDazed;
    public bool IsShielding;
    bool PlayerIsDead;

    // boss variables
    bool CanBattle = true;
    bool IsFortressAttack;
    bool IsBossBattle;
    bool IsMagicAttack;
    int bossChargeCountdown = 2;
    bool IsCharging = false;
    int bossCharge = 2;
    int round;

    [SerializeField] MAINmanager mainManager;
    [SerializeField] BattleVisuals battleVisuals;
    EnemyManager enemyManager;
    [SerializeField] Death death;

    Tutorial tutorial;

    [SerializeField] GameObject playerGO;
    Attributes playerAttributes;
    PlayerLevelManager playerLevelManager;
    FogOfWar fogOfWar;

    GameObject enemyGO;
    Attributes enemyAttributes;

    public List<SpellObjects> PlayerCurrentBuffs;
    public List<SpellObjects> EnemyCurrentBuffs;
    public List<SpellObjects> playerCurrentSkills;

    string health = "health";

    int enemyAttack;
    int enemyDefence;
    int playerAttack;
    int playerDefence;

    int finalEnemyHit;

    List<SpellObjects> objectsToDeleteEnemy;
    List<SpellObjects> objectsToDeletePlayer;
    List<SpellObjects> skillsToDeletePlayer;

    void Start()
    {
        enemyManager = mainManager.GetEnemyManager();
        tutorial = mainManager.GetTutorial();

        playerAttributes = playerGO.GetComponent<Attributes>();
        playerLevelManager = playerGO.GetComponent<PlayerLevelManager>();
        fogOfWar = playerGO.GetComponent<FogOfWar>();

        objectsToDeleteEnemy = new List<SpellObjects>();
        objectsToDeletePlayer = new List<SpellObjects>();
        skillsToDeletePlayer = new List<SpellObjects>();
    }

    void Update()
    {
        if (IsBattling)
        {
            OnBattleStart?.Invoke(true);
            if (!attacking && CanBattle)
            {
                currentCoroutine = StartCoroutine(BattleTurn());
            }
        }   
    }

    IEnumerator BattleTurn()
    {
        attacking = true;
        IsDazed = false;
        IsShielding = false;
        canUseResource = true;

        battleVisuals.PlayerAttackAnimation();

        if(IsBossBattle) BossAttack();
        else battleVisuals.EnemyAttackAnimation(IsBossBattle);

        StartCoroutine(WaitForSound());

        yield return new WaitForSeconds(2f);

        if (objectsToDeleteEnemy.Count > 0)  DeleteBuffs(ref EnemyCurrentBuffs, objectsToDeleteEnemy, enemyAttributes); 
        if (objectsToDeletePlayer.Count > 0) DeleteBuffs(ref PlayerCurrentBuffs, objectsToDeletePlayer, playerAttributes);

        attacking = false;

        CalculateAttack();

        objectsToDeleteEnemy = new List<SpellObjects>();
        objectsToDeletePlayer = new List<SpellObjects>();
        skillsToDeletePlayer = new List<SpellObjects>();

        CheckActiveBuffs();

        if(playerCurrentSkills.Count > 0) WaitForSkillCooldown(); 
        if (skillsToDeletePlayer.Count > 0) DeleteSkills(skillsToDeletePlayer);

        if (!IsMagicAttack && !IsCharging) CalculateFinalEnemyAttack();
        int finalPlayerHit = ((playerAttack - enemyDefence) < 0) ? 0 : playerAttack - enemyDefence;

        if (!IsDazed && !IsShielding) playerAttributes.SetValue(-finalEnemyHit, health);
        else finalEnemyHit = 0;        

        if (IsMagicAttack)
        {
            StartCoroutine(WaitForBossMagicAnim(finalPlayerHit));
            IsMagicAttack = false;
        }
        else
        {
            enemyAttributes.SetValue(-finalPlayerHit, health);
            battleVisuals.RoundVisualsBoth(-finalEnemyHit, -finalPlayerHit);
        }

        if (playerAttributes.GetHealth() <= 0) DestroyCharacter(playerGO);  
        else if (enemyAttributes.GetHealth() <= 0 && enemyGO != null && IsBattling) DestroyCharacter(enemyGO);
        

        finalEnemyHit = 0;
        finalPlayerHit = 0;
    }
    void CalculateAttack()
    {
        enemyAttack = enemyAttributes.baseAttack + UnityEngine.Random.Range(enemyAttributes.weaponAttack.x, enemyAttributes.weaponAttack.y + 1);
        playerAttack = playerAttributes.baseAttack + UnityEngine.Random.Range(playerAttributes.weaponAttack.x, playerAttributes.weaponAttack.y + 1);

        enemyDefence = enemyAttributes.baseDefence + UnityEngine.Random.Range(enemyAttributes.armourDefence.x, enemyAttributes.armourDefence.y + 1);
        playerDefence = playerAttributes.baseDefence + UnityEngine.Random.Range(playerAttributes.armourDefence.x, playerAttributes.armourDefence.y + 1);
    }
    void CalculateFinalEnemyAttack()
    {
        finalEnemyHit = ((enemyAttack - playerDefence) < 0) ? 0 : enemyAttack - playerDefence;
        if (finalEnemyHit <= 0) finalEnemyHit = 1;
    }
    void CheckActiveBuffs()
    {

        if (EnemyCurrentBuffs.Count > 0)
        {
            UseBuffs(ref EnemyCurrentBuffs, objectsToDeleteEnemy, enemyAttributes);
        }
        if (PlayerCurrentBuffs.Count > 0)
        {
            UseBuffs(ref PlayerCurrentBuffs, objectsToDeletePlayer, playerAttributes);
        }
    }

    public void StartBattle(GameObject enemyGO)
    {
        IsBattling = true;
        canUseResource = true;
        fogOfWar.ClearCircle();

        this.enemyGO = enemyGO;
        this.enemyAttributes = enemyGO.GetComponent<Attributes>();

        if(!IsFortressAttack && !IsBossBattle) SoundSystem.instance.ChangeMusic(SoundSystem.Music.Battle);
        if(tutorial.IsInTutorial) tutorial.HideTutorial();

        mainManager.ChooseScriptActivity(false);

        enemyManager.DeactivateEnemiesDuringbattle(enemyGO);
        battleVisuals.SetEnemyGO(enemyGO);
        battleVisuals.StartBattleSetUp();
    }
    public void StartBattle(GameObject enemyPrefab, bool IsFortressAttack)
    {
        GameObject newEnemy = Instantiate(enemyPrefab);
        enemyManager.SetEnemyManagers(ref newEnemy);

        StartBattle(newEnemy);
    }

    public void DestroyCharacter(GameObject character)
    {
        EnemyCurrentBuffs.Clear();
        PlayerCurrentBuffs.Clear();
        playerCurrentSkills.Clear();
        
        battleVisuals.AddBuffs(PlayerCurrentBuffs, battleVisuals.buffIconListPlayer);
        battleVisuals.AddBuffs(EnemyCurrentBuffs, battleVisuals.buffIconListEnemy);
        if (character.GetComponent<EnemyObjects>())
        {
            SoundSystem.instance.PlaySound(SoundSystem.Sound.EnemyDeath);
            enemyManager.RemoveEnemy(character);
            playerLevelManager.SetCurrentXP(character.GetComponent<EnemyObjects>().GetXPAmount());
        }
        else SoundSystem.instance.PlaySound(SoundSystem.Sound.Death);
            IsBattling = false;
        attacking = false;

        StopCoroutine(currentCoroutine);
        StartCoroutine(WaitForDeathAnim(character));
    }
    IEnumerator WaitForDeathAnim(GameObject character)
    {
        battleVisuals.DeathAnimation(character);
        canUseResource = false;

        float duration = (round == 2) ? 2f : 1.5f;
        if(round == 2) round = 0;

        yield return new WaitForSeconds(duration);
    
        EndBattle(ref character);
    }
    public void EndBattle(ref GameObject character)
    {
        if (!IsFortressAttack) battleVisuals.EndBattleSetUp();

        if (character != playerGO) Destroy(character);
        else
        {
            OnPlayerDeath();
        }

        mainManager.ChooseScriptActivity(true);
        if (!IsFortressAttack && character != playerGO)
        {
            SoundSystem.instance.PlaySound(SoundSystem.Sound.Win);
            SoundSystem.instance.ChangeMusic(SoundSystem.Music.Main);
        }
        if (character == playerGO)
        {
            SoundSystem.instance.ChangeMusic(SoundSystem.Music.Death);
        }
        enemyManager.ActivateEnemies();

        if (tutorial.IsInTutorial) tutorial.RevealTutorial();
        this.enemyGO = null;
        character = null;
        OnBattleStart?.Invoke(false);
    }

    public bool GetIsBattling()
    {
        return IsBattling;
    }

    int GetValueFromResourcePull(BattleResourcesManager.ResourcePullType resourcePullType, float percentage, Attributes currentAttributes)
    {
        int value = 0;
        if (resourcePullType == BattleResourcesManager.ResourcePullType.Mana)
        {
            // chANGE TO - vvalue
            value = (int)(currentAttributes.maxMana * (percentage / 100));
        }
        else if (resourcePullType == BattleResourcesManager.ResourcePullType.Health)
        {
            value = (int)(currentAttributes.maxHealth * (percentage / 100));
        }
        else if (resourcePullType == BattleResourcesManager.ResourcePullType.Attack)
        {
            value = (int)(currentAttributes.baseAttack * (percentage / 100));
        }
        else if(resourcePullType == BattleResourcesManager.ResourcePullType.Defence)
        {
            value = (int)(currentAttributes.baseDefence * (percentage / 100));
        }

        return value;
    }

    public void DamageEnemy(BattleResourcesManager.ResourcePullType resourcePullType, float percentage)
    {

        int value = GetValueFromResourcePull(resourcePullType, percentage, playerAttributes);

        enemyAttributes.SetValue(-value, health);
        battleVisuals.RoundVisualsBoth(damageToPlayer: 0, damageToEnemy: -value);

        if (enemyAttributes.GetHealth() <= 0) DestroyCharacter(enemyGO);
    }


    // receives a buff from potion or spell and returns if it deals damage or gives health
    public int OnBuffUsed(SpellObjects spellObject, BattleResourcesManager.ResourcePullType resourcePullType, Attributes currentAttributes)
    {
        int value = GetValueFromResourcePull(resourcePullType, spellObject.GetBuff().buffPercentage, currentAttributes);


        if (spellObject.GetBuff().buffType == BuffTypeSO.BuffTypes.Healing)
        {
            return value;
        }
        else if (spellObject.GetBuff().buffType == BuffTypeSO.BuffTypes.Damaging)
        {
            return -value;
        }
        return 0;
    }
    void UseBuffs(ref List<SpellObjects> currentSpellList, List<SpellObjects> deleteList, Attributes currentAttributes)
    {
        foreach (SpellObjects spellObject in currentSpellList)
        {
            SelectBuffType(spellObject);

            spellObject.currentBuffLength--;

            if (spellObject.GetBuff().buffType != BuffTypeSO.BuffTypes.Boost)
            {
                int amount = 0;
                if (spellObject.GetSpellType() == SpellTypesSO.SpellType.Fire)
                    amount = OnBuffUsed(spellObject, spellObject.GetResourcePullType(), playerAttributes);
                else if (spellObject.GetSpellType() == SpellTypesSO.SpellType.Bleeding)
                    amount = OnBuffUsed(spellObject, spellObject.GetResourcePullType(), enemyAttributes);

                    currentAttributes.SetValue(amount, health);


                if (currentAttributes == playerAttributes)
                {
                    battleVisuals.RoundVisualsBoth(damageToPlayer: amount, damageToEnemy: 0);
                }
                else
                {
                    battleVisuals.RoundVisualsBoth(damageToPlayer: 0, damageToEnemy: amount);
                }
            }

            if (spellObject.currentBuffLength <= 0)
            {
                spellObject.currentBuffLength = spellObject.buffLength;
                deleteList.Add(spellObject);
            }
        }
        if (currentAttributes == enemyAttributes) battleVisuals.AddBuffs(currentSpellList, battleVisuals.buffIconListEnemy);
        else if (currentAttributes == playerAttributes) battleVisuals.AddBuffs(currentSpellList, battleVisuals.buffIconListPlayer);
    }
    void SelectBuffType(SpellObjects spellObject)
    {
        IsDazed = (spellObject.GetSpellType() == SpellTypesSO.SpellType.Lightning) ? true : false;
        IsShielding = (spellObject.GetSpellType() == SpellTypesSO.SpellType.Shield) ? true : false;

        enemyAttack = (spellObject.GetSpellType() == SpellTypesSO.SpellType.Poisoning) ? enemyAttack - spellObject.GetBuffAmount(enemyAttributes) : enemyAttack;
        enemyDefence = (spellObject.GetSpellType() == SpellTypesSO.SpellType.DefenceDecrease) ? enemyDefence - spellObject.GetBuffAmount(enemyAttributes) : enemyDefence;
        enemyAttack = (spellObject.GetSpellType() == SpellTypesSO.SpellType.Translucent) ? enemyAttack - spellObject.GetBuffAmount(playerAttributes) : enemyAttack;
        playerAttack = (spellObject.GetSpellType() == SpellTypesSO.SpellType.AttackBoost) ? playerAttack + spellObject.GetBuffAmount(playerAttributes) : playerAttack;
        playerDefence = (spellObject.GetSpellType() == SpellTypesSO.SpellType.DefenceBoost) ? playerDefence + spellObject.GetBuffAmount(playerAttributes) : playerDefence;
        playerDefence = (spellObject.GetSpellType() == SpellTypesSO.SpellType.EndBossMagicAttack) ? playerDefence - spellObject.GetBuffAmount(playerAttributes) : playerDefence;
    }
    void DeleteBuffs(ref List<SpellObjects> currentSpellList, List<SpellObjects> deleteList, Attributes currentAttributes)
    {
        foreach (SpellObjects spellObject in deleteList)
        {
            if (spellObject.GetSpellType() == SpellTypesSO.SpellType.Lightning)
                IsDazed = false;
            
            currentSpellList.Remove(spellObject);
        }

        if (currentAttributes == enemyAttributes) battleVisuals.RemoveBuffs(currentSpellList: currentSpellList, battleVisuals.buffIconListEnemy);
        else if (currentAttributes == playerAttributes) battleVisuals.RemoveBuffs(currentSpellList: currentSpellList, currentIconList: battleVisuals.buffIconListPlayer);
    }

    void WaitForSkillCooldown()
    {
        foreach (SpellObjects skillObject in playerCurrentSkills)
        {
            skillObject.DecreaseCooldown();
            battleVisuals.SkillCooldownVisuals(skillObject, IsDeleting: false);

            if (skillObject.currentSkillCooldown <= 0)
            {
                skillObject.currentSkillCooldown = skillObject.GetCooldown();
                skillsToDeletePlayer.Add(skillObject);
                battleVisuals.SkillCooldownVisuals(skillObject, IsDeleting: true);
            }
        }
    }
    IEnumerator WaitForSound()
    {
        yield return new WaitForSeconds(1.5f);

        SoundSystem.instance.PlaySound(SoundSystem.Sound.Attack);

    }

    void DeleteSkills(List<SpellObjects> listToDelete)
    {
        foreach (SpellObjects skillObject in listToDelete)
        {
            playerCurrentSkills.Remove(skillObject);
        }
    }

    public void SetCanBattle(bool canBattle)
    {
        this.CanBattle = canBattle;
    }
    public bool GetPlayerDead()
    {
        return PlayerIsDead;
    }
    public void OnPlayerDeath()
    {
        death.OnDeath();
        Destroy(enemyGO);
        battleVisuals.EndBattleSetUp();
        PlayerIsDead = true;
        StopFortressAttack();

        SetIsFortressAttack(false, 0);
    }
    public bool GetIsFortressAttack()
    {
        return IsFortressAttack;
    }
    public void SetIsFortressAttack(bool option, int round)
    {
        IsFortressAttack = option;
        this.round = round;
    }
    public void StartBossBattle()
    {
        IsBossBattle = true;
    }
    public bool GetIsBossBattle()
    {
        return IsBossBattle;
    }
    public bool GetIsCharging()
    {
        return IsCharging;
    }
    public void BossAttack()
    {
        bossChargeCountdown--;

        if(bossChargeCountdown < 0) {
            IsCharging = true;
            bossCharge--;

            battleVisuals.SetIsMagicAttack(true);

            if(bossCharge <= 0)
            {

                // balance
                finalEnemyHit = 40 + (int)(playerAttributes.GetMaxHealth() * .1f);
                IsMagicAttack = true;
                battleVisuals.SetIsMagicAttack(true);
                IsCharging = false;

                bossCharge = 2;
                bossChargeCountdown = 2;
            }
            else if(bossCharge > 0)
            {
                Debug.Log("in");
                finalEnemyHit = 0;
                battleVisuals.EnemyAttackAnimation(true);
            }
        }
        else
        {
            battleVisuals.EnemyAttackAnimation(false);

        }
    }
    IEnumerator WaitForBossMagicAnim(int damage)
    {
        battleVisuals.BossMagicAttack();

        yield return new WaitForSeconds(.8f);
        
        battleVisuals.DamagePlayer();


        enemyAttributes.SetValue(-damage, health);
        battleVisuals.RoundVisualsBoth(-finalEnemyHit, -damage);
        battleVisuals.SetIsMagicAttack(false);


    }
    public void StopBossAttack()
    {
        bossCharge = 2;
        bossChargeCountdown = 2;
        IsMagicAttack = false;
        IsCharging = false;

        battleVisuals.SetIsMagicAttack(false);
        battleVisuals.StopEnemyMagicAttack();
        Debug.Log("stopping boss attack!!");
    }
    public void StopFortressAttack()
    {
        IsBattling = false;
        IsFortressAttack = false;
        attacking = false;
        IsBossBattle = false;
    }
}
