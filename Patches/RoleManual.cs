
using System.Collections.Generic;

namespace TownOfUs.Roles {
    public class RoleManual {
        public static readonly Dictionary<RoleEnum, string> roleManual = new Dictionary<RoleEnum, string>() {
            {RoleEnum.Sheriff, "Sheriff\nキルボタンを持っており、第三陣営、インポスターをキルすることができますが、対象がクルーメイトだった場合は自身が死亡します。"},
            {RoleEnum.Police, "Sheriff\nキルボタンを持っており、第三陣営、インポスターをキルすることができますが、対象がクルーメイトだった場合は自身が死亡します。"},
            {RoleEnum.Jester, "Jester\n会議により追放されると勝利します。 ベントの使用、死体の移動、変身の能力を持ちます。"},
            {RoleEnum.Engineer, "Engineer\nゲーム中一度だけ、サボタージュをどこからでも即座に修理できるFixボタンを持っています。\nベントが使用できます。"},
            {RoleEnum.LoverImpostor, "LoverImpostor\n"},
            {RoleEnum.Lover, "Lover\n"},
            {RoleEnum.Mayor, "Mayor\n最多得票者が複数いた場合、Mayorに投票されたクルーが追放されます。\nどこにいても緊急会議を開けるボタンを持っています。\n緊急会議ボタンが未使用のときにキルされると、即座に緊急会議が開かれます。"},
            {RoleEnum.Zombie, "Zombie\nインポスターの人数が0または全体の半数以上となったときに、自身のタスクが完了していれば単独勝利します。\nキルされても一定時間後に蘇生します。Seerに占われると死亡します。\n蘇生前に会議が始まるか、追放されるか、Seerに占われて死亡した場合は蘇生しません。"},
            {RoleEnum.Swapper, "Swapper\n会議中に指定した二人の得票を入れ替えます。各プレイヤーに対してそれぞれ一度しか対象に選べません。\nまた、緊急会議ボタンを使用できません。"},
            {RoleEnum.Investigator, "Investigator\n一定時間ごとに、マップに自身の周辺のクルーの位置と色が表示されます。カモフラージュ状態や障害物に関わらず表示され、対象が死体であった場合、感知できる距離が二倍になります。"},
            {RoleEnum.TimeLord, "TimeLord\n全プレイヤーの時間を数秒間巻き戻すことができます。その時間内に死亡していたクルーは蘇生されます。\nまた、バイタル情報を見ることができません。"},
            {RoleEnum.Painter, "Painter\n床にペンキを塗ります。塗ったペンキは会議を挟んだ後全プレイヤーに視認されるようになり、その上を通過したクルーのバイザーの色を変化させます。\nこの色は次の会議後まで全てのプレイヤーが視認でき、会議が終わると元に戻ります。"},
            {RoleEnum.Shifter, "Shifter\n"},
            {RoleEnum.Medic, "Medic\nゲーム中一回、他のクルーに対してキルを一度だけ防ぐシールドを張ることができます。" +
                             " シールドを張られたクルーメイトにキルが行われると、Medicの画面が緑色に点灯します。\n" +
                             "死体のReport時、キルから発見までの経過時間に応じてチャット欄に\n" +
                             "キルしたクルーの名前やキルしたクルーの色の明暗の情報を得られます。"},
            {RoleEnum.Seer, "Seer\n占いによってクルーメイトの陣営を知ります。占うためには一定時間触れている必要があり、その間自身は移動できません。\nZombieを占った場合、Zombieが死亡します。"},
            {RoleEnum.Executioner, "Executioner\nゲーム開始時に指定されるターゲットが会議で追放されると勝利します。ターゲットが追放以外で死亡した場合、役職がJesterまたはCrewmateになります。"},
            {RoleEnum.Spy, "Spy\n"},
            {RoleEnum.Sniffer, "Sniffer\n死体が近くにあるほど周辺が赤く変化します。\nAdmin情報を見ることと、死体の通報を行うことができません。"},
            {RoleEnum.Snitch, "Snitch\n自身のタスクを完了させることによって誰がインポスターかを知ることができます。\nPolus、Airshipでは、閉じられたドアに触れるだけでドアを開くことができます。"},
            {RoleEnum.Charger, "Charger\n電力を消費して視界とAdmin情報をパワーアップさせます。\n電力はベントに入ることで充電できますが、他のベントへの移動はできません。\n充電が切れた場合、Admin情報を見ることができなくなります。\nAdmin閲覧中は消費電力が3倍になります。"},
            {RoleEnum.Druid, "Druid\n死体を一定距離移動させることで蘇生させます。\n距離は、死体を運び始めた位置からどれだけ離れたかによって判定されます。"},
            {RoleEnum.SecurityGuard, "SecurityGuard\nゲーム開始時に所持しているネジを使用し、新しいカメラの設置、ベントの封鎖ができます。設置されたオブジェクトは次の会議が開けてから視認され、有効になります。"},
            {RoleEnum.Arsonist, "Arsonist\n生存している自分以外のすべてのクルーに油を塗った後にIgniteすると勝利します。 油を塗るためには3秒間接触している必要があります。"},
            {RoleEnum.Altruist, "Altruist\n自身の命と引換えに、クルーの死体を蘇生させます。蘇生には一定時間が必要になり、成功すると自分自身は死体を残さず消えます。"},
            {RoleEnum.Phantom, "Phantom\n死亡したインポスター以外のプレイヤーがPhantomとなる可能性があります。Phantomは会議後にランダムなベントから発生し、すべてのタスクを完了させると勝利します。\n生存しているクルーによってクリックされると死亡します。"},
            {RoleEnum.Sniper, "Sniper\n一度の会議で二人の役職を当てて狙撃すると勝利します。\n役職を間違った場合、自身が死亡します。\n広い視界を持ちます。"},
            {RoleEnum.Miner, "Miner\n一定個数のベントを作ることができます。新たなベントと直前に作成したベントがそれぞれ接続されます。"},
            {RoleEnum.Swooper, "Swooper\n一定時間姿を消します。その間に再度使用することで姿を表すこともできます。\nベントは使用できません。"},
            {RoleEnum.Morphling, "Morphling\nサンプルを取得した対象の姿に変身することができます。\n変身中に再度ボタンを押すことで変身を解除できます。\nベントは使用できません。"},
            {RoleEnum.Puppeteer, "Puppeteer\n数秒かけて接触している相手に憑依します。\n憑依されたクルーは、次に近付いた他のクルーをキルします。\nキルが発生するかReleaseボタンにより憑依は解除されます。\nPuppeteerは、憑依している間と憑依解除後の3秒間は行動が行なえません。"},
            {RoleEnum.Camouflager, "Camouflager\n"},
            {RoleEnum.Janitor, "Janitor\n数秒間死体に隣接することで死体を消し、発見されなくします。"},
            {RoleEnum.Kirby, "Popopo\n死体を吸い込んだり吐き出したりします。吸い込んでいる間は対象の姿をコピーします。\nベントは使用できません。"},
            {RoleEnum.Undertaker, "Undertaker\n死体を移動させることができます。"},
            {RoleEnum.Assassin, "AssassinまたはMad Mate\n会議中にクルーの役職を当てることで狙撃し、キルすることができます。\nMad Mateの場合、クルーメイトとして数えられ、味方インポスターが誰かを知らず、通常キルとベントの使用ができません。"},
            {RoleEnum.Underdog, "Underdog\n"},
            {RoleEnum.MultiKiller, "MultiKiller\nキルクールが二倍になる代わりに、5秒以内であれば二人目を連続でキルできます。"},
            {RoleEnum.Cracker, "Cracker\n次に他の誰かが入ったときに停電する罠を部屋全体に仕掛けます。\n停電中の部屋ではタスク、マップ、通報が使用不可になります。\n罠が設置された部屋は、解除されるまでAdmin情報に人数が映らなくなります。\nSkeldの廊下に仕掛けた場合、すべての廊下が停電状態になります。\n別の部屋でCrackを使用した場合、すでに設置されていたCrackは解除されます。"},
            {RoleEnum.DollMaker, "DollMaker\nキルの代わりにクルーを蝋人形状態にします。\n蝋人形になったクルーは行動できなくなり、一定時間が経過するか、他のプレイヤーに触れられるか、ミーティングが始まると死亡します。"},
            {RoleEnum.Glitch, "Glitch\n誰でもキルすることができ、自分以外のクルーが全員死亡したときのみ勝利となります。\n任意の対象に変身するMimic、対象の能力やタスクを一定時間行えなくするHackが使用できます。\nベントが使用できます。"},
            {RoleEnum.Crewmate, "Crewmate\n特別な能力を持たないクルーメイトです。"},
            {RoleEnum.Impostor, "Impostor\n特別な能力を持たないインポスターです。"},
        };
    }
}