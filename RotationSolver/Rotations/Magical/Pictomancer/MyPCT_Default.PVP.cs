namespace RotationSolver.Rotations.Magical.Pictomancer;

[Rotation("My Custom PCT PvP", CombatType.PvP, GameVersion = "7.58", RotationType = RotationType.Reborn)]

public class PCT_DefaultPvP : PictomancerRotation
{
	#region Configurations
	[Range(0, 1, ConfigUnitType.Percent)]
	[RotationConfig(CombatType.PvP, Name = "Health threshold needed to use Tempura Coat")]
	public float TempuraThreshold { get; set; } = 0.8f;

	[RotationConfig(CombatType.PvP, Name = "Freely use burst damage oGCDs")]
	public bool FreeBurst { get; set; } = true;

	[Range(0, 1, ConfigUnitType.Percent)]
	[RotationConfig(CombatType.PvP, Name = "Enemy HP threshold needed to use burst oGCDs on if previous config disabled")]
	public float BurstThreshold { get; set; } = 0.55f;
	#endregion

	// 敵をターゲットしていて25m以内にいるかどうかの判定
	private bool HasValidTarget => CurrentTarget != null && CurrentTarget.DistanceToPlayer() <= 25f;

	// イマジン・ポンを今すぐ撃つべきか
	private bool CanUsePomNow =>
		HasValidTarget &&
		(FreeBurst || CurrentTarget?.GetHealthRatio() <= BurstThreshold) &&
		PomMusePvP.CanUse(out _, usedUp: true, skipAoeCheck: true);

	#region oGCDs
	// 0手目（魔法を撃つ前）に強制的にイマジン・ポンを発動させる
	protected override bool EmergencyAbility(IAction nextGCD, out IAction? action)
	{
		if (CanUsePomNow)
		{
			if (PomMusePvP.CanUse(out action, usedUp: true, skipAoeCheck: true))
			{
				return true;
			}
		}

		return base.EmergencyAbility(nextGCD, out action);
	}

	protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? action)
	{
		if (TemperaCoatPvP.CanUse(out action) && Player?.GetHealthRatio() <= TempuraThreshold)
		{
			return true;
		}

		return base.DefenseSingleAbility(nextGCD, out action);
	}

	protected override bool AttackAbility(IAction nextGCD, out IAction? action)
	{
		if (CanUsePomNow)
		{
			if (PomMusePvP.CanUse(out action, usedUp: true, skipAoeCheck: true))
			{
				return true;
			}
		}

		if (RustPvP.CanUse(out action, skipAoeCheck: true))
		{
			return true;
		}

		if (PhantomDartPvP.CanUse(out action))
		{
			return true;
		}

		if (FreeBurst || CurrentTarget?.GetHealthRatio() <= BurstThreshold)
		{
			if (WingedMusePvP.CanUse(out action, usedUp: true, skipAoeCheck: true))
			{
				return true;
			}

			if (ClawedMusePvP.CanUse(out action, usedUp: true, skipAoeCheck: true))
			{
				return true;
			}

			if (FangedMusePvP.CanUse(out action, usedUp: true, skipAoeCheck: true))
			{
				return true;
			}
		}

		switch (IsMoving)
		{
			case true:
				if (ReleaseSubtractivePalettePvP.CanUse(out action))
				{
					return true;
				}

				break;
			case false:
				if (SubtractivePalettePvP.CanUse(out action))
				{
					return true;
				}

				break;
		}

		return base.AttackAbility(nextGCD, out action);
	}
	#endregion

	#region GCDs
	protected override bool GeneralGCD(out IAction? action)
	{
		// イマジン・ポンが撃てる状態（バフをつける前）なら、魔法を一切撃たずに待機してイマジン・ポンを先に出させる
		if (CanUsePomNow)
		{
			action = null;
			return false;
		}

		// イマジン・ポンが出た直後、チャージがある限りブラックコメットを2連発する！
		if (CometInBlackPvP.CanUse(out action, usedUp: true, skipAoeCheck: true))
		{
			return true;
		}

		if (CometPvP.CanUse(out action, skipAoeCheck: true))
		{
			return true;
		}

		if (StarPrismPvP.CanUse(out action, skipAoeCheck: true))
		{
			return true;
		}

		if (MogOfTheAgesPvP.CanUse(out action, skipAoeCheck: true))
		{
			return true;
		}

		if (RetributionOfTheMadeenPvP.CanUse(out action, skipAoeCheck: true))
		{
			return true;
		}

		if (PomMotifPvP.CanUse(out action))
		{
			return true;
		}

		if (WingMotifPvP.CanUse(out action))
		{
			return true;
		}

		if (ClawMotifPvP.CanUse(out action))
		{
			return true;
		}

		if (MawMotifPvP.CanUse(out action))
		{
			return true;
		}

		if (FireInRedPvP.CanUse(out action))
		{
			return true;
		}

		return base.GeneralGCD(out action);
	}
	#endregion
}
