
using System.Collections.Generic;

namespace TownOfUs.Roles {
    public static class RoleManual {
        public static readonly Dictionary<RoleEnum, string> roleManual = new Dictionary<RoleEnum, string> {
            {RoleEnum.Mayor, $"Mayor\n最多得票者が複数いた場合、Mayorに投票されたクルーが追放されます。\nどこにいても緊急会議を開けるボタンを持っています。\n緊急会議ボタンが未使用のときにキルされると、即座に緊急会議が開かれます。\n" +
                             $"ボタンを未使用でキルされた際に即座に緊急会議が開かれる {(CustomGameOptions.MayorMeetingOnDead? "On": "Off")}"},
            {RoleEnum.BodyGuard, $"BodyGuard\n他のプレイヤーを一定時間護衛します。矢印が表示される距離以内にいる間、護衛されているプレイヤーに行われたキルを防ぎ、代わりに死亡します。\n" +
                               $"そのとき、護衛されたプレイヤーの画面が緑色に点灯します。\n" +
                               $"護衛のクールダウン時間	 {CustomGameOptions.GuardCoolDown}s\n" +
                               $"護衛の継続時間	 {CustomGameOptions.GuardDuration}s\n" +
                               $"護衛の有効距離	 {CustomGameOptions.GuardRange}m\n" +
                               $"護衛成功時に自身が死亡する {(CustomGameOptions.DieOnGuard? "On": "Off")}\n" +
                               $"護衛通知が表示されるプレイヤー {new[] {"Everyone", "Shielded", "Nobody"}[(int)CustomGameOptions.NotificationShield]}"},
            {RoleEnum.Sheriff, $"Sheriff\nキルボタンを持っており、第三陣営、インポスターをキルすることができますが、対象がクルーメイトだった場合は自身が死亡します。\n" +
                               $"対象がクルーメイトでもキルする {(CustomGameOptions.SheriffKillOther? "On": "Off")}\n" +
                               $"Madmateもキル対象に含まれる {(CustomGameOptions.SheriffKillsMadmate? "On": "Off")}\n" +
                               $"キルのクールダウン時間	 {CustomGameOptions.SheriffKillCd}s\n" +
                               $"自身でキルした相手をReportできる {(CustomGameOptions.SheriffBodyReport? "On": "Off")}"},
            {RoleEnum.Engineer, $"Engineer\nベントを利用でき、サボタージュを即座に修理できるFixボタンを持っています。\nFixボタンはベントの中でのみ使用できます。\n" +
                                $"Fixボタンが会議を挟むごとに使用できる	{(CustomGameOptions.EngineerFixPer == EngineerFixPer.Round? "On": "Off")}\n" +
                                $"ベントの中でのみFixボタン使用できる   {(CustomGameOptions.EngineerCanFixOnlyInVent? "On": "Off")}"},
            {RoleEnum.Jester, $"Jester\n会議により追放されると勝利します。 ベントの使用、死体の移動、変身の能力を持ちます。\n" +
                              $"ベントを使用できる	{(CustomGameOptions.JesterUseVent? "On": "Off")}\n" +
                              $"死体を移動できる	{(CustomGameOptions.JesterDragBody? "On": "Off")}\n" +
                              $"変身できる	{(CustomGameOptions.JesterCanMorph? "On": "Off")}"},
            {RoleEnum.LoverImpostor, $"LoverImpostor\n"},
            {RoleEnum.Lover, $"Lover\n"},
            {RoleEnum.Zombie, $"Zombie\nインポスターの人数が0または全体の半数以上となったときに、自身のタスクが完了していれば単独勝利します。\n" +
                              $"キルされても一定時間後に蘇生します。Seerに占われると死亡します。蘇生前に会議が始まるか、追放されるか、Seerに占われて死亡した場合は蘇生しません。\n" +
                              $"蘇生するまでの時間   {CustomGameOptions.ZombieReviveTime}s\n" +
                              $"Seerに占われたときに死亡する	{(CustomGameOptions.ZombieKilledBySeer? "On": "Off")}"},
            {RoleEnum.Swapper, $"Swapper\n会議中に指定した二人の得票を入れ替えます。各プレイヤーに対してそれぞれ一度しか対象に選べません。\nまた、緊急会議ボタンを使用できません。"},
            {RoleEnum.Investigator, $"Investigator\n一定時間ごとに、マップに自身の周辺のクルーの位置と色が表示されます。カモフラージュ状態や障害物に関わらず表示され、対象が死体であった場合、感知できる距離が二倍になります。また、マップを開いている間は移動ができません。\n" +
                              $"マップにクルーが表示される距離	{CustomGameOptions.InvestigatorSeeRange}m\n" +
                              $"クルーの色まで識別できる距離（表示距離に対する割合） {CustomGameOptions.InvestigatorSeeColorRange}%\n" +
                              $"マップ情報が更新される頻度   {CustomGameOptions.InvestigatorMapUpdate}s"},
            {RoleEnum.TimeLord, $"TimeLord\n全プレイヤーの時間を数秒間巻き戻すことができます。その時間内に死亡していたクルーは蘇生されます。\nまた、バイタル情報を見ることができません。\n" +
                              $"時間内に死亡していたクルーが蘇生する	{(CustomGameOptions.RewindRevive? "On": "Off")}\n" +
                              $"巻き戻し中、青いアウトラインがつく	{(CustomGameOptions.RewindFlash? "On": "Off")}\n" +
                              $"巻き戻される時間の秒数	        {CustomGameOptions.RewindDuration}s\n" +
                              $"巻き戻すボタンのクールダウン時間	{CustomGameOptions.RewindCooldown}s"},
            {RoleEnum.Painter, $"Painter\n床にペンキを塗ります。塗ったペンキは会議を挟んだ後全プレイヤーに視認されるようになり、その上を通過したクルーのバイザーの色を変化させます。\nこの色は次の会議後まで全てのプレイヤーが視認でき、会議が終わると元に戻ります。\n" +
                              $"塗れるペンキの色の数	{CustomGameOptions.PaintColorMax}\n" +
                              $"ペイントのクールダウン時間	{CustomGameOptions.PaintCd}s"},
            {RoleEnum.Shifter, $"Shifter\n\n" +
                              $""},
            {RoleEnum.Medic, $"Medic\n自身が死体をReportした時、会議時に容疑者候補の名前がグレーで表示されます。\n" +
                             $"容疑者候補の数はキルからレポートまでの時間が短いほど少なくなり、最短でランダムで2人まで絞り込まれます。\n" + 
                             $"容疑者候補が増える間隔	{CustomGameOptions.MedicReportDegradation}s"},
            {RoleEnum.Seer, $"Seer\n占いによってクルーメイトの陣営を知ります。占うためには一定時間移動せずに触れている必要があります。\n" +
                            $"占われた場合、クルーメイトは緑、インポスターは赤く名前が表示されます。Zombieを占った場合、Zombieが死亡します。\n" +
                              $"占いのクールダウン時間	{CustomGameOptions.SeerCd}s\n" +
                              $"占うために必要な接触時間	{CustomGameOptions.SeerInvestigateTime}s\n" +
                              $"占いで得られる情報	{new[] {"陣営", "役職"}[(int)CustomGameOptions.SeerInfo]}\n" +
                              $"占われたことを知るプレイヤー	{new[] {"Nobody", "Imps+Neut", "Crew", "All"}[(int)CustomGameOptions.SeeReveal]}\n" +
                              $"第三陣営の占い結果をインポスターと同じにする	{(CustomGameOptions.NeutralRed? "On": "Off")}"},
            {RoleEnum.Executioner, $"Executioner\nゲーム開始時に指定されるターゲットが会議で追放されると勝利します。ターゲットが追放以外で死亡した場合、役職がJesterになります。"},
            {RoleEnum.Spy, $"Spy\n\n" +
                              $""},
            {RoleEnum.Sniffer, $"Sniffer\n死体が近くにあるほど周辺が赤く変化します。\nAdmin情報を見ることと、死体の通報を行うことができません。\n" +
                              $"死体を通報できるか	{(CustomGameOptions.SnifferCanReport? "On": "Off")}\n" +
                              $"死体を感知できる範囲	{CustomGameOptions.SnifferMaxRange}m\n"},
            {RoleEnum.Snitch, $"Snitch\nPolus、Airshipでは、閉じられたドアに触れるだけでドアを開くことができます。\n" +
                              $"自身のタスクを完了させることによって誰がインポスターかを知ることができます。\n" +
                              $"残りのタスクが１個以下になると、インポスターから自身の役職と場所が知られます。\n" +
                              $"第三陣営の情報も得る	{(CustomGameOptions.SnitchSeesNeutrals? "On": "Off")}\n" +
                              $"会議中に撃たれなくなる条件	{new[] {"残りタスクが１個以下", "タスク完了"}[(int)CustomGameOptions.SnitchShotTiming]}\n" +
                              $"ドアを即座に開ける	{new[] {"常時", "残りタスクが１個以下のときのみ", "Off"}[(int)CustomGameOptions.SnitchOpenDoorImmediately]}"},
            {RoleEnum.Charger, $"Charger\n電力を消費して視界とAdmin情報をパワーアップさせます。\n" +
                               $"電力はベントに入ることで充電できますが、他のベントへの移動はできません。\n" +
                               $"充電が切れた場合、Admin情報を見ることができなくなります。\n" +
                               $"Admin閲覧中は消費電力が3倍になります。\n" +
                               $"最大まで充電するのにかかる時間    	{CustomGameOptions.MaxChargeTime}s\n" +
                               $"電力をすべて消費するのにかかる時間	{CustomGameOptions.ConsumeChargeTime}s"},
            {RoleEnum.Druid, $"Druid\n死体を一定距離遠くまで移動させることで蘇生させます。\n" +
                              $"蘇生に必要な距離	{CustomGameOptions.DruidReviveRange}m\n" +
                              $"ラウンドごとの蘇生可能回数		{CustomGameOptions.DruidReviveLimit}"},
            {RoleEnum.SecurityGuard, $"SecurityGuard\nゲーム開始時に所持しているネジを使用し、新しいカメラの設置、ベントの封鎖ができます。\n" +
                                     $"設置されたオブジェクトは次の会議が開けてから視認され、有効になります。\n" +
                                     $"Skeld、MiraHQではカメラを設置することはできません。\n" +
                              $"カメラ設置、ベント封鎖のクールダウン時間	{CustomGameOptions.SecurityGuardCooldown}\n" +
                              $"ゲーム開始時に所持しているネジの個数	{CustomGameOptions.SecurityGuardTotalScrews}\n" +
                              $"カメラの設置に必要なネジの個数	{CustomGameOptions.SecurityGuardCamPrice}\n" +
                              $"ベントの封鎖に必要なネジの個数	{CustomGameOptions.SecurityGuardVentPrice}"},
            {RoleEnum.Arsonist, $"Arsonist\n生存している自分以外のすべてのクルーに油を塗った後にIgniteすると勝利します。 油を塗るためには一定時間接触している必要があります。\n" +
                              $"油を塗るのに必要な接触時間	{CustomGameOptions.ArsonistDouseTime}s\n" +
                              $"油を塗るクールダウン時間	{CustomGameOptions.DouseCd}s"},
            {RoleEnum.Necromancer, $"Necromancer\n死んだクルーを次の会議が終わるまで生きた状態にします。"},
            {RoleEnum.Phantom, $"Phantom\n死亡したインポスター以外のプレイヤーがPhantomとなる可能性があります。Phantomは会議後にランダムなベントから発生し、すべてのタスクを完了させると勝利します。\n" +
                               $"生存しているクルーによってクリックされると死亡します。"},
            {RoleEnum.Sniper, $"Sniper\n一度の会議で二人の役職を当てて狙撃すると勝利します。\n役職を間違った場合、自身が死亡します。\n広い視界を持ちます。\n" +
                              $"勝利のために必要な狙撃回数	{CustomGameOptions.SniperWinCnt}"},
            {RoleEnum.Miner, $"Miner\n一定個数のベントを作ることができます。新たなベントと直前に作成したベントがそれぞれ接続されます。\n" +
                              $"ベント作成のクールダウン時間	{CustomGameOptions.MineCd}s\n" +
                              $"作成できる最大ベント数	{CustomGameOptions.MaxVentNum}"},
            {RoleEnum.Swooper, $"Swooper\n一定時間姿を消し、移動速度を上げます。その間に再度使用することで中断できます。\n" +
                              $"姿を消す最大継続時間	{CustomGameOptions.SwoopDuration}s\n" +
                              $"姿を消すクールダウン時間	{CustomGameOptions.SwoopCd}s\n" +
                              $"姿を消している間の移動速度	{CustomGameOptions.SwooperVelocity}%\n" +
                              $"ベント使用         {(new[] {"能力使用中は不可", "常時可能", "不可"}[(int)CustomGameOptions.SwooperCanVent])}"},
            {RoleEnum.Morphling, $"Morphling\nサンプルを取得した対象の姿に変身することができます。\n変身中に再度ボタンを押すことで変身を解除できます。\n変身中はベントを使用できません。\n" +
                              $"変身のクールダウン	{CustomGameOptions.MorphlingCd}s\n" +
                              $"変身の継続時間		{CustomGameOptions.MorphlingDuration}s\n" + 
                              $"ベント使用         {(new[] {"変身中は不可", "常時可能", "不可"}[(int)CustomGameOptions.MorphCanVent])}"},
            {RoleEnum.Puppeteer, $"Puppeteer\n一定時間かけて接触している相手に憑依し、次に近付いた他のクルーをキルさせます。\n" + 
                                 "キルが発生するかReleaseボタンにより憑依は解除され、その後数秒間は行動が行えません。\n" +
                                 "憑依によりキルしたクルーは、自身がキルした対象をReportできません。\n" +
                              $"憑依に必要な接触時間	{CustomGameOptions.PossessTime}s\n" +
                                 $"憑依し続けられる最大時間	{CustomGameOptions.PossessMaxTime}s\n" +
                                 $"憑依解除後の行動不能時間	{CustomGameOptions.ReleaseWaitTime}s\n" +
                                $"憑依キルした対象をReportできる	{(CustomGameOptions.PossessBodyReport? "On": "Off")}\n"},
            {RoleEnum.Camouflager, $"Camouflager\n互いのクルーを識別できなくなるカモフラージュ状態を発生させることができます。\n" +
                              $"カモフラージュ能力のクールダウン時間	{CustomGameOptions.CamouflagerCd}s" +
                              $"カモフラージュ状態の継続時間	{CustomGameOptions.CamouflagerDuration}s"},
            {RoleEnum.Janitor, $"Janitor\n数秒間死体に隣接することで死体を消し、発見されなくします。\n" +
                              $"死体を消せるクールダウン時間	{CustomGameOptions.CleanCd}s\n" +
                              $"死体を消すのにかかる時間	{CustomGameOptions.CleanDuration}s"},
            {RoleEnum.Kirby, $"Popopo\n死体を吸い込んだり吐き出したりします。吸い込んでいる間は対象の姿をコピーします。\n" +
                             $"連続コピー可能時間	{CustomGameOptions.CopyDuration}\n" +
                             $"ベント使用         {(new[] {"能力使用中は不可", "常時可能", "不可"}[(int)CustomGameOptions.KirbyCanVent])}"},
            {RoleEnum.Undertaker, $"Undertaker\n死体を移動させることができます。\n" +
                              $"死体を置いてから次に移動させられるまでのクールダウン時間	{CustomGameOptions.DragCd}s\n" +
                              $"死体移動時の通常時に対する移動速度の割合	{CustomGameOptions.DragVel}%\n" +
                              $"死体移動中のベント移動	{(CustomGameOptions.VentWithBody? "On": "Off")}"},
            {RoleEnum.Assassin, $"AssassinまたはMadmate\n会議中にクルーの役職を当てることで狙撃し、キルすることができます。\n" +
                                $"Madmateの場合、クルーメイトとして数えられ、味方インポスターが誰かを知らず、通常キルとベントの使用ができません。\n" +
                              $"会議中にキルできる最大人数  {CustomGameOptions.AssassinKills}\n" +
                                $"一度の会議中に二人以上狙撃できる	{(CustomGameOptions.AssassinMultiKill? "On": "Off")}\n" +
                                $"最後の一人となったインポスターは狙撃能力を得る   {(CustomGameOptions.LastImpCanGuess? "On": "Off")}"},
            {RoleEnum.Underdog, $"Underdog\n\n" +
                              $""},
            {RoleEnum.MultiKiller, $"MultiKiller\nキルクールが二倍になる代わりに、5秒以内であれば二人目を連続でキルできます。\n" +
                              $"通常のクールダウンに対するキルクールダウン時間の倍率	{CustomGameOptions.MultiKillerCdRate}%\n" +
                              $"二人目を連続でキルできる秒数	{CustomGameOptions.MultiKillEnableTime}s\n"},
            {RoleEnum.Cracker, $"Cracker\nサボタージュでドアを閉めた部屋を停電させます。\n" +
                               $"同時に、一定時間タスク、マップ、通報が使用不可になり、Admin情報に人数が映らなくなります。\n" +
                               $"これらの効果はCrackerが生存していれば別のImpostorがドアを閉めた場合も発生します。\n" +
                               $"停電の継続時間	{CustomGameOptions.BlackoutDur}s\n" +
                               $"クラック効果の継続時間	{CustomGameOptions.CrackDur}s"},
            {RoleEnum.DollMaker, $"DollMaker\nキルの代わりにクルーを蝋人形状態にします。\n" +
                                 $"蝋人形になったクルーは行動できなくなり、一定時間が経過するか、他のプレイヤーに触れられるか、ミーティングが始まると死亡します。\n" +
                              $"蝋人形にされたクルーが自動で死亡するまでの時間	{CustomGameOptions.DollBreakTime}s\n" +
                                 $"他のクルーが触れると蝋人形が死亡する   {(CustomGameOptions.DollBreakOnTouch? "On": "Off")}"},
            {RoleEnum.Glitch, $"Glitch\n誰でもキルすることができ、自分以外のクルーが全員死亡したときのみ勝利となります。" +
                              $"変身、対象の能力やタスクを一定時間行えなくするHack、ベント、どこでもAdminが使用できます。\n" +
                              $"どこでもAdminの利用	{(CustomGameOptions.GlitchAdmin? "On": "Off")}\n" +
                              $"Mimic能力のクールダウン時間	{CustomGameOptions.MimicCooldown}s\n" +
                              $"Mimic能力の効果時間	{CustomGameOptions.MimicDuration}s\n" +
                              $"Hack能力のクールダウン時間	{CustomGameOptions.HackCooldown}s\n" +
                              $"Hack能力の効果時間	{CustomGameOptions.HackDuration}s\n" +
                              $"キルクールダウン時間	{CustomGameOptions.GlitchKillCooldown}s"},
            {RoleEnum.Crewmate, $"Crewmate\n特別な能力を持たないクルーメイトです。"},
            {RoleEnum.Impostor, $"Impostor\n特別な能力を持たないインポスターです。"},
        };
    }
}