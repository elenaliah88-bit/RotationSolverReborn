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

	// イマジン・ポンが今使える（＝これからバフをつける）状態かどうかの判定
	private bool CanUsePomNow =>
		(FreeBurst || CurrentTarget?.GetHealthRatio() <= BurstThreshold) &&
		PomMusePvP.CanUse(out _, usedUp: true);

	#region oGCDs
	protected override bool EmergencyAbility(IAction nextGCD, out IAction? action)
	{
		if (CanUsePomNow)
		{
			if (PomMusePvP.CanUse(out action, usedUp: true))
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
			if (PomMusePvP.CanUse(out action, usedUp: true))
			{
				return true;
			}
		}

		if (RustPvP.CanUse(out action))
		{
			return true;
		}

		if (PhantomDartPvP.CanUse(out action))
		{
			return true;
		}

		if (FreeBurst || CurrentTarget?.GetHealthRatio() <= BurstThreshold)
		{
			if (WingedMusePvP.CanUse(out action, usedUp: true))
			{
				return true;
			}

			if (ClawedMusePvP.CanUse(out action, usedUp: true))
			{
				return true;
			}

			if (FangedMusePvP.CanUse(out action, usedUp: true))
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
		// 【重要】すでに戦闘中（InCombat）で、これからイマジン・ポンを撃つ瞬間だけ他の魔法を止める
		if (InCombat && CanUsePomNow)
		{
			action = null;
			return false;
		}

		// イマジン・ポンが残っている間（CanUsePomNowがtrueの間）は、ブラックコメットを温存する！
		if (!CanUsePomNow && CometInBlackPvP.CanUse(out action, usedUp: true))
		{
			return true;
		}

		if (CometPvP.CanUse(out action))
		{
			return true;
		}

		if (StarPrismPvP.CanUse(out action))
		{
			return true;
		}

		if (MogOfTheAgesPvP.CanUse(out action))
		{
			return true;
		}

		if (RetributionOfTheMadeenPvP.CanUse(out action))
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
