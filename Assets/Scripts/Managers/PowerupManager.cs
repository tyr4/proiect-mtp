using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
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
            _playerPowerups.Add(new OwnedPowerup(powerup, 1));
            AssignPowerup(powerup);
            
            return;
        }

        if (owned.Base is IHasTiers)
            owned.CurrentTier++;
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

            return !(Contains(_playerPowerups, p) && tier >= 3);
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
        if (powerup is not IHasTiers || _playerPowerups.IsNullOrEmpty()) return 0;
        
        var found = _playerPowerups.Find(p => p.Base == powerup);

        return found?.CurrentTier ?? 0;
    }

    private bool Contains(List<OwnedPowerup> list, Powerup powerup)
    {
        if (list.IsNullOrEmpty()) return false;
        
        return list.Find(p => p.Base == powerup) != null;
    }
    
    public OwnedPowerup Find(Powerup powerup)
    {
        if (_playerPowerups.IsNullOrEmpty()) return null;
        
        return _playerPowerups.Find(p => p.Base == powerup);
    }
}
