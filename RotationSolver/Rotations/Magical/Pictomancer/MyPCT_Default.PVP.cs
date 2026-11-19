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

	// ターゲットがいて、イマジン・ポンが撃てる状態か（余計な距離計算を外してシンプルに判定）
	private bool CanUsePomNow =>
		CurrentTarget != null &&
		(FreeBurst || CurrentTarget.GetHealthRatio() <= BurstThreshold) &&
		PomMusePvP.CanUse(out _, usedUp: true, skipAoeCheck: true);

	#region oGCDs
	// 最優先枠でイマジン・ポンを叩き込む
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
		// 【戦闘中の最優先ブレーキ】イマジン・ポンが残っているなら、あらゆる魔法を一瞬止めてイマジン・ポンを先に出させる
		if (InCombat && CanUsePomNow)
		{
			action = null;
			return false;
		}

		// 【ブラックコメット保護】イマジン・ポンが撃てる状態（CanUsePomNow == true）の間は、ブラックコメットを絶対に温存する！
		if (!CanUsePomNow && CometInBlackPvP.CanUse(out action, usedUp: true, skipAoeCheck: true))
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
