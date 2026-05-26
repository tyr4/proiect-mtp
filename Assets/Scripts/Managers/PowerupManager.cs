using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PowerupManager : MonoBehaviour
{
    [SerializeField] private PowerupContainer allPowerups;
    public static PowerupManager Instance;

    private List<OwnedPowerup> _playerPowerups = new();
    private List<OwnedPowerup> _powerupsThisSelection = new();
    
    private Powerup _randomPowerup;

    public static event Action<OwnedPowerup> OnPowerupAdded;
    public static event Action<OwnedPowerup> OnPowerupUpdated;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        PowerupPanelContainerUI.OnPowerupSelected += OnPowerupSelectClick;
    }
    
    private void OnDisable()
    {
        PowerupPanelContainerUI.OnPowerupSelected -= OnPowerupSelectClick;
    }

    private void OnPowerupSelectClick(Powerup powerup)
    {
        UpdatePlayerPowerups(powerup);
    }

    public void UpdatePlayerPowerups(Powerup powerup)
    {
        var owned = _playerPowerups.Find(p => p.Base == powerup);
        
        if (owned == null)
        {
            var newPowerup = new OwnedPowerup(powerup, 1);
            _playerPowerups.Add(newPowerup);

            AssignPowerup(powerup);
            powerup.OnSelect(newPowerup);
            
            OnPowerupAdded?.Invoke(newPowerup);
            
            return;
        }
        
        owned.CurrentTier++;
        powerup.OnSelect(owned);
        
        OnPowerupUpdated?.Invoke(owned);
    }

    // assign the powerup to the correct manager
    private void AssignPowerup(Powerup powerup)
    {
        powerup.OnAssign();
            
         // if (powerup is Projectile proj)
        // {
        //     ProjectileManager.Instance.Register(proj);
        // }
    }

    public void RemovePlayerPowerup(OwnedPowerup powerup)
    {
        _playerPowerups.Remove(powerup);
    }

    public List<OwnedPowerup> GetPlayerPowerups()
    {
        return _playerPowerups;
    }

    public List<Powerup> GetAllPowerups()
    {
        return allPowerups.Powerups;
    }

    public List<OwnedPowerup> GeneratePowerupChoices()
    {
        _powerupsThisSelection.Clear();
        
        // all valid powerups that arent owned and maxed (omits one time buffs that are tier
        // independent)
        var valid = allPowerups.Powerups.Where(p =>
        {
            var tier = GetPlayerPowerupTier(p);

            return !(Contains(_playerPowerups, p) && (tier >= 3 && p is not OneTimeBuff));
        }).ToList();
        
        for (int i = 0; i < 3; i++)
        {
            var randomIndex = Random.Range(0, valid.Count);
            _randomPowerup = valid[randomIndex];
            var tier = GetPlayerPowerupTier(_randomPowerup);

            _powerupsThisSelection.Add(new OwnedPowerup(_randomPowerup, tier));
            valid.RemoveAt(randomIndex);
        }
        
        return _powerupsThisSelection;
    }
    
    private int GetPlayerPowerupTier(Powerup powerup)
    {
        var found = _playerPowerups.Find(p => p.Base == powerup);

        return found?.CurrentTier ?? 0;
    }

    private bool Contains(List<OwnedPowerup> list, Powerup powerup)
    {
        if (list.Count == 0) return false;
        
        return list.Find(p => p.Base == powerup) != null;
    }
    
    // these 2 functions have to be separate because of external callers not knowing
    // about the lists
    public OwnedPowerup FindPlayerPowerup(Powerup powerup)
    {
        // this is the only time it can return null if the powerup first goes through
        // UpdatePlayerPowerups()
        if (_playerPowerups.Count == 0) return null;
        
        return _playerPowerups.Find(p => p.Base == powerup);
    }
    
    private Powerup FindAllPowerup<T>() where T : Powerup
    {
        if (allPowerups.Powerups.Count == 0) return null;
        
        return allPowerups.Powerups.Find(p => p is T);
    }
    
    // TODO: pass some player type enum?
    public void AssignDefaultPowerup()
    {
        var powerup = FindAllPowerup<Bow>();
        
        UpdatePlayerPowerups(powerup);
    }
}
