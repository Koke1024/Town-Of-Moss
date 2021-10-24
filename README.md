# Town of Moss

当MODは [Town of Us](https://github.com/polusgg/Town-Of-Us)
を個人的に改変した、[Among Us](https://www.innersloth.com/games/among-us/) の非公式MODです。\
蘇生に対応したAutoMuteUsのCaptureツールも公開しています。

## Releases

### Town of Moss

| Among Us - Version| Mod Version | Link |
|----------|-------------|-----------------|
| 2021.6.30s | v0.372 | [Download](https://spiel.jp/mod/Moss0372.zip) |
| 2021.6.30s | v0.37 | [Download](https://spiel.jp/mod/Moss037.zip) |
| 2021.6.30s | v0.363 ※動作安定 | [Download](https://spiel.jp/mod/Moss0363.zip) |
| 2021.6.30s | v0.362 | [Download](https://spiel.jp/mod/Moss0362.zip) |
| 2021.6.30s | v0.352 | [Download](https://spiel.jp/mod/Moss0352.zip) |
| 2021.6.30s | v0.35 | [Download](https://spiel.jp/mod/Moss035.zip) |
| 2021.6.30s | v0.34 | [Download](https://spiel.jp/mod/Moss034.zip) |
| 2021.6.30s | v0.33 | [Download](https://spiel.jp/mod/Moss033.zip) |
| 2021.6.30s | v0.32 | [Download](https://spiel.jp/mod/Moss032.zip) |

### AUCapture for MOD

| Version | Link |
|----------|-----------------|
| v1.00 | [Download](https://spiel.jp/mod/AUCapture_MOD.zip) |

AUCapture for MODは[AmongUsCapture](https://github.com/automuteus/amonguscapture) を元に非公式に作成しています。\
ゲーム中の蘇生に対応していますが、追放直後の一度だけミュート解除されないことがあるようです。

<details>
    <summary> Changelog </summary>
    <details>
        <summary> v0.372 </summary>
        <ul> 
            <li>Janitorのデフォルトのクールダウンを長く</li>
            <li>Puppeteerの憑依解除後の硬直時間を3秒に固定</li>
            <li>Puppeteerのクールダウン時間をキルクールダウン時間と共通に</li>
        </ul>
    </details>
    <details>
        <summary> v0.371 </summary>
        <ul> 
            <li>採用役職が少ない場合に本来追加される役職が減る不具合を修正</li>
            <li>インポスターの採用役職が少ない場合にゲームが開始できない不具合を修正</li>
            <li>新規役職がsniperの対象になかった問題を修正</li>
            <li>SecurityGuardのカメラ設置が無効なSkeldで使用されないように</li>
        </ul>
    </details>
    <details>
        <summary> v0.37 </summary>
        <ul> 
            <li>DollMaker追加</li>
        </ul>
    </details>
    <details>
        <summary> v0.363 </summary>
        <ul> 
            <li>Bug Fix</li>
            <li>Kirbyベント使用不可に</li>
        </ul>
    </details>
    <details>
        <summary> v0.352 </summary>
            <ul> <li>Bug Fix</li> </ul>
        </details>
        <details>
            <summary> v0.35 </summary>
            <ul> <li>Bug Fix</li> </ul>
        </details>
        <details>
            <summary> v0.33 </summary>
        <ul>
            <li>新役職Kirby追加</li>
            <li>新役職Zombie追加</li>
            <li>Mayor能力変更</li>
        </ul>
    </details>
</details>

-----------------------

|**Crewmate Roles**|**Impostor Roles**|**Neutral Roles**|**Modifiers**|
|----------|-------------|----------|-----|
|[Mayor](#Mayor)|[Janitor](#Janitor)|[Glitch](#Glitch)|[Torch](#Torch)|
|[Sheriff](#Sheriff)|[Morphling](#Morphling)|[Jester](#Jester)|[Diseased](#Diseased)|
|[Engineer](#Engineer)|[Camouflager](#Camouflager)|[Executioner](#Executioner)|[Flash](#Flash)|
|[Swapper](#Swapper)|[Miner](#Miner)|[Arsonist](#Arsonist)|[Giant](#Giant)|
|[Investigator](#Investigator)|[Swooper](#Swooper)|[Phantom](#Phantom)||
|[Time Lord](#TimeLord)|[Assassin](#Assassin)|[Sniper](#Sniper)||
|[Medic](#Medic)|[Undertaker](#Undertaker)|[Zombie](#Zombie)||
|[Seer](#Seer)|[Kirby](#Kirby)|||
|[SecurityGuard](#SecurityGuard)|[Cracker](#Cracker)|||
|[Snitch](#Snitch)|[MultiKiller](#MultiKiller)|||
|[Altruist](#Altruist)|[Puppeteer](#Puppeteer)|||
|[Charger](#Charger)|[DollMaker](#DollMaker)|||
|[Druid](#Druid)||||

-----------------------

# Roles

# Crewmate Roles

## Mayor

### **Team: Crewmates**

最多得票者が複数いた場合、Mayorに投票されたクルーが追放されます。 \
どこにいても緊急会議を開けるボタンを持っています。\
緊急会議ボタンが未使用のときにキルされると、即座に緊急会議が開かれます。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Call Meeting On Dead | キルされた際に緊急会議を開く | Toggle | True |

-----------------------

## Sheriff

### **Team: Crewmates**

キルボタンを持っており、第三陣営、インポスターをキルすることができますが、対象がクルーメイトだった場合は自身が死亡します。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Show Sheriff | 自身の役職が全員に公開される | Toggle | False |
| Sheriff Miskill Kills Crewmate | クルーメイトもキルする | Toggle | False |
| Sheriff Kills Mad Mate | Mad Mateもキルできる対象に含む | Toggle | False |
| Sheriff Kill Cooldown | キルのクールダウン時間 | Time | 25s |
| Sheriff can report who they've killed | 自身でキルした相手をReportできる | Toggle | True |

-----------------------

## Engineer

### **Team: Crewmates**

ゲーム中一度だけ、サボタージュをどこからでも即座に修理できるFixボタンを持っています。\
ベントが使用できます。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Engineer Fix Per | Roundの場合、会議を行うごとにFixボタンが使用できる | Round / Game | Game |

-----------------------

## Swapper

### **Team: Crewmates**

会議中に指定した二人の得票を入れ替えます。各プレイヤーに対してそれぞれ一度しか対象に選べません。\
また、緊急会議ボタンを使用できません。

-----------------------

## Investigator

### **Team: Crewmates**

一定時間ごとに、マップに自身の周辺のクルーの位置と色が表示されます。カモフラージュ状態や障害物に関わらず表示され、対象が死体であった場合、感知できる距離が二倍になります。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| See Someone Range | マップにクルーが表示される距離 | Number | 6.0m |
| See Color Range | クルーの色まで識別できる距離（See Someone Rangeに対する割合） | Number | 30% |
| Map Update Interval | マップ情報が更新される頻度 | Number | 2.0s |

-----------------------

## TimeLord

### **Team: Crewmates**

全プレイヤーの時間を数秒間巻き戻すことができます。その時間内に死亡していたクルーは蘇生されます。\
また、バイタル情報を見ることができません。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Revive During Rewind | 時間内に死亡していたクルーが蘇生する | Toggle | True |
| Rewind Duration | 巻き戻される時間の秒数 | Time | 3s |
| Rewind Cooldown | 巻き戻すボタンのクールダウン時間 | Time | 25s |
| Time Lord can use Vitals | バイタルを見れるかどうか | Toggle | False |

-----------------------

## Medic

### **Team: Crewmates**

ゲーム中一回、他のクルーに対してキルを一度だけ防ぐシールドを張ることができます。 シールドを張られたクルーメイトにキルが行われると、Medicの画面が緑色に点灯します。\
死体のReport時、キルから発見までの経過時間に応じてチャットから以下の追加情報を得られます。

- キルしたクルーの名前
- キルしたクルーの色が暗いか明るいか

| 暗い|明るい|
|----|:-----:|
|<span style="color:Red">Red</span>|<span style="color:Pink">Pink</span>|
|<span style="color:Blue">Blue</span>|<span style="color:Orange">Orange</span>|
|<span style="color:Green">Green</span>|<span style="color:Yellow">Yellow</span>|
|<span style="color:Black">Black</span>|<span style="color:White">White</span>|
|<span style="color:Purple">Purple</span>|<span style="color:Cyan">Cyan</span>|
|<span style="color:Brown">Brown</span>|<span style="color:Lime">Lime</span>|
|<span style="color:Maroon">Maroon</span>|<span style="color:Rose">Rose</span>|
|<span style="color:Tan">Tan</span>|<span style="color:Banana">Banana</span>|
|<span style="color:Watermelon">Watermelon</span>|<span style="color:Gray">Gray</span>|
|<span style="color:Chocolate">Chocolate</span>|<span style="color:Coral">Coral</span>|
|<span style="color:Beige">Beige</span>|<span style="color:Sky Blue">Sky Blue</span>|
| |<span style="color:Hot Pink">Hot Pink</span>|
| |<span style="color:Turquoise">Turquoise</span>|
| |<span style="color:Lilac">Lilac</span>|
| |<span style="color:Rainbow">Rainbow</span>|
| |<span style="color:Azure">Azure</span>|

------------------------

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Show Shielded Player | 誰にシールドが見えるか | Self / Medic / Self + Medic / Everyone | Medic |
| Show Medic Reports | 死体発見時に追加情報を得る | Toggle | True |
| Time Where Medic Reports Will Have Name | 追加情報でキルしたクルーの名前が見られるまでの発見時間 | Time | 0s |
| Time Where Medic Reports Will Have Color Type | 追加情報でキルしたクルーの色の濃さがわかるまでの発見時間 | Time | 15s |
| Who gets murder attempt indicator | シールドを張られたクルーにキルが試みられた際の点灯が見えるプレイヤー | Medic / Shielded / Everyone / Nobody | Medic |
| Shield breaks on murder attempt | シールドが一度のキルにより破壊されるかどうか | Toggle | True |

-----------------------

## Seer

### **Team: Crewmates**

占いによってクルーメイトの陣営を知ります。占うためには一定時間触れている必要があり、その間自身は移動できません。\
Zombieを占った場合、Zombieが死亡します。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Seer Cooldown | 占いを行うクールダウン時間 | Time | 25s |
| Seer Investigating Time | 占いを行うために必要な接触時間 | Time | 3s |
| Info that Seer sees | 占いによって得られる情報 | Role / Team | Team |
| Who Sees That They Are Revealed | 占われたことを知るプレイヤー | Crewmates / Impostors + Neutral / All / Nobody | None |
| Neutrals show up as Impostors | 第三陣営をインポスターと同じ表示にする | Toggle | True |

-----------------------

## SecurityGuard

### **Team: Crewmates**

ゲーム開始時に所持している7つのネジによって、新しいカメラの設置、ベントの封鎖ができます。

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Number Of Screw | ゲーム開始時に所持しているネジの個数 | Number Of Screw | 7 |
| Number Of Screws Per Cam | カメラの設置に必要なネジの個数 | Number | 2 |
| Number Of Screws Per Vent | ベントの封鎖に必要なネジの個数 | Number | 1 |

-----------------------

## Snitch

### **Team: Crewmates**

自身のタスクを完了させることによって誰がインポスターかを知ることができます。\
Polus、Airshipでは、閉じられたドアに触れるだけでドアを開くことができます。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Sees Neutral Roles | 第三陣営も知れるかどうか | Toggle | False |
| Open door immediately | いつドアを即座に開けられるか | Always / One Task Left / None | Always |

-----------------------

## Altruist

### **Team: Crewmates**

自身の命と引換えに、クルーの死体を蘇生させます。蘇生には一定時間が必要になります。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Revive Duration | 蘇生に必要な時間 | Time | 1.0s |
| Target's body disappears | 蘇生中、蘇生対象の死体を見えなくする | Toggle | False |

-----------------------

## Charger

### **Team: Crewmates**

電力を消費して視界とAdmin情報をパワーアップさせます。\
電力はベントに入ることで充電できますが、他のベントへの移動はできません。\
充電が切れた場合、Admin情報を見ることができなくなります。\
Admin閲覧中は消費電力が3倍になります。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Time to Charge Maximum | 最大まで充電するのにかかる時間 | Time | 1.0s |
| Time to Consume | 電力をすべて消費するのにかかる時間 | Time | 30.0s |

-----------------------

## Druid

### **Team: Crewmates**

死体を一定距離移動させることで蘇生させます。\
距離は、死体を拾った位置からどれだけ離れたかによって判定されます。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Distance to revive dead | 蘇生に必要な距離 | Time | 20.0m |

-----------------------

# Impostor Roles

## Assassin

### **Team: Impostors**

会議中に相手の役職を当てることで狙撃し、キルすることができます。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Number of Assassin Kill | 会議中にキルできる最大人数 | Number | 5 |
| Assassin Can Kill Continuous  | 一度の会議中に二人以上狙撃できる | Toggle | True |
| Last Impostor Can Snipe  | 最後の一人となったインポスターは狙撃能力を得る | Toggle | True |

-----------------------

## Janitor

### **Team: Impostors**

3秒間死体に隣接することで死体を消し、発見されなくします。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Clean Cooldown | 死体を消せるクールダウン時間 | Time | 45s |
| Clean Duration | 死体を消すのにかかる時間 | Time | 3.0s |

-----------------------

## Morphling

### **Team: Impostors**

サンプルを取得した対象の姿に変身することができます。\
変身中に再度ボタンを押すことで変身を解除できます。\
ベントは使用できません。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Morph Cooldown | 変身のクールダウン | Time | 15s |
| Morph Duration | 変身の継続時間 | Time | 10s |

-----------------------

## Camouflager

### **Team: Impostors**

互いのクルーを識別できなくなるカモフラージュ状態を発生させることができます。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Camouflage Cooldown | カモフラージュ能力のクールダウン時間 | Time | 25s |
| Camouflage Duration | カモフラージュ状態の継続時間 | Time | 5s |

-----------------------

## Miner

### **Team: Impostors**

ベントを作ることができます。新たなベントと直前に作成したベントがそれぞれ接続されます。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Mine Cooldown | ベント作成のクールダウン時間 | Time | 20s |
| Max Mine Num | 作成できる最大ベント数 | Number | 5 |

-----------------------

## Swooper

### **Team: Impostors**

一定時間姿を消します。その間に再度使用することで姿を表すこともできます。\
ベントは使用できません。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Swooper Cooldown | 姿を消すクールダウン時間 | Time | 30s |
| Swooper Duration | 姿を消す最大継続時間 | Time | 5s |

-----------------------

## Undertaker

### **Team: Impostors**

死体を移動させることができます。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Drag Cooldown | 死体を置いてから次に移動させられるまでのクールダウン時間 | Time | 1s |
| Drag Velocity | 死体移動時の通常時に対する移動速度の割合 | Percent | 100% |

-----------------------

## Cracker

### **Team: Impostors**

次に他の誰かが入ったときに停電する罠を部屋全体に仕掛けます。\
停電中の部屋ではタスク、マップ、通報が使用不可になります。\
罠が設置された部屋は、解除されるまでAdmin情報に人数が映らなくなります。\
Skeldの廊下に仕掛けた場合、すべての廊下が停電状態になります。\
別の部屋でCrackを使用した場合、すでに設置されていたCrackは解除されます。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Crack Cooldown | クラック能力のクールダウン時間 | Time | 25s |
| Crack Duration | クラック発動時の停電の継続時間 | Time | 10s |

-----------------------

## MultiKiller

### **Team: Impostors**

キルクールが二倍になる代わり、5秒以内であれば二人目を連続でキルできます。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| MultiKiller Cooldown Rate | 通常のクールダウンに対するキルクールダウン時間の長さ | Percent | 200% |
| MultiKill Enable Time | 二人目を連続でキルできる秒数 | Time | 5.0s |

-----------------------

## Kirby

### **Team: Impostors**

死体を吸い込んだり吐き出したりします。吸い込んでいる間は対象の姿をコピーします。 ベントは使用できません。

-----------------------

## Puppeteer

### **Team: Impostors**

3秒間かけて接触している相手に憑依します。\
憑依されたクルーは、次に近付いた他のクルーをキルします。\
キルが発生するかReleaseボタンにより憑依は解除されます。\
憑依している間と憑依解除後の3秒間は移動ができなくなります。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Possess Time | 憑依に必要な接触時間 | Time | 3s
| Possess Max Time | 憑依し続けられる最大時間  | Time | 15s
| Wait Time After Release | 憑依解除後の行動不能時間  | Time | 3s

## DollMaker

### **Team: Impostors**

キルの代わりにクルーを蝋人形状態にします。\
蝋人形になったクルーは行動できなくなり、一定時間が経過するか、他のプレイヤーに触れられるか、ミーティングが始まると死亡します。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Doll Self Broken Time | 蝋人形にされたクルーが自動で死亡するまでの時間 | Time | 20s

-----------------------

# Neutral Roles

## Glitch

### **Team: Neutral**

誰でもキルすることができ、自分以外のクルーが全員死亡したときのみ勝利となります。\
任意の対象に変身するMimic、対象の能力やタスクを一定時間行えなくするHackが使用できます。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Add Glitch | チェックを入れた場合、Neutral Rolesの数と関係なく、必ずGlitchが誰かに割り当てられます。 | Toggle | False |
| Mimic Cooldown | Mimic能力のクールダウン時間 | Time | 10s |
| Mimic Duration | Mimic能力の効果時間 | Time | 15s |
| Hack Cooldown | Hack能力のクールダウン時間 | Time | 10s |
| Hack Duration | Mimic能力の効果時間 | Time | 15s |
| Glitch Kill Cooldown | キルクールダウン時間 | Time | 30s |
| Glitch Hack Distance | Hack能力の射程範囲 | Short / Normal / Long | Short |

-----------------------

## Jester

### **Team: Neutral**

会議により追放されると勝利します。 ベントの使用、死体の移動、変身の能力を持ちます。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Jester Can Use Vents | ベントを使用できる | Toggle | True |
| Jester Can Drag Body | 死体を移動できる | Toggle | True |
| Jester Can Morph | 変身できる | Toggle | True |

-----------------------

## Executioner

### **Team: Neutral**

ゲーム開始時に指定されるターゲットが会議で追放されると勝利します。\
ターゲットが追放以外で死亡した場合、役職がJesterになります。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Executioner becomes on Target Dead | ターゲットが追放以外で死亡した場合に変更される自身の役職 | Crewmate / Jester | Jester |

-----------------------

## Arsonist

### **Team: Neutral**

生存している自分以外のすべてのクルーに油を塗った後にIgniteすると勝利します。 油を塗るためには3秒間接触している必要があります。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Douse Duration | 油を塗るのに必要な接触時間 | Time | 3.0s |
| Douse Cooldown | 油を塗るクールダウン時間 | Time | 15s |

-----------------------

## Phantom

### **Team: Neutral**

陣営に関係なく、死亡したプレイヤーがPhantomとなる可能性があります。Phantomは会議後にランダムなベントから発生し、すべてのタスクを完了させると勝利します。\
生存しているクルーによってクリックされると死亡します。

-----------------------

## Sniper

### **Team: Neutral**

一度の会議で二人の役職を当てて狙撃すると勝利します。\
役職を間違った場合、自身が死亡します。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Number of Sniper Kills to Win | 勝利のために必要な狙撃回数 | Number | 2 |

-----------------------

## Zombie

### **Team: Neutral**

インポスターの残数が０になるか、インポスターの数が半数となったとき、自身のタスクが完了していれば単独勝利します。\
キルされても一定時間後に蘇生しますが、Seerに占われた場合は蘇生しません。

### Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Zombie Revive Time | 蘇生するまでの時間 | Time | 15s |
| Killed By Seer | 占われたときに死亡する | Toggle | True |

-----------------------

# Modifiers

ロールとは別に追加される能力です。

## Torch

### **Applied to: Crewmates**

停電中も視界が広くなります。

-----------------------

## Diseased

### **Applied to: Crewmates**

キルされた相手のクールダウンを三倍にします。

-----------------------

## Flash

### **Applied to: All**

足が速くなります。

-----------------------

## Giant

### **Applied to: All**

大きくなります。

-----------------------

# Custom Game Options

| Name | Description | Type | Default |
|----------|:-------------:|:------:|:------:|
| Add Assassin As Mad Mate |インポスターとは別に、狙撃能力を持つAssassinがMad Mateとして追加されます。この場合、通常のインポスターとしてのAssassinは登場しません。狂人はインポスター陣営ですが、通常キルはできず、クルー陣営の人数としてカウントされます。他のインポスターからはクルーと同様に見え、キルすることもできます。SeerでTeamを占った場合、クルー（緑色）として表示されます。SheriffはMad Mateをキルできません。 | Toggle | False |
| Add Glitch | グリッチが追加されます。 | Toggle | False |
| Number of Neutral Roles | 割り当てられる第三陣営の数 | Number | 1 |
| Kill Cooldown Reset on Meeting | 会議の後にインポスターのキルクールダウンを初期化するかどうか | Toggle | False |
| Camouflaged Comms | コミュニケーションサボタージュ中、カモフラージュ状態になる | Toggle | True |
| Impostors can see the roles of their team | インポスター同士が互いの役職を知っている | Toggle | True |
| Polus Reactor Time Limit | PolusのMelt Downサボタージュの制限時間 | Time | 45.0s |
| Polus Vital Move | PolusのVitalの位置を変更できる。Shipにした場合Drop Ship内にベントが一つ追加される | Default / Labo / Ship / O2 | Default |
| Admin Has Usable Limit Time | AdminのRoundごとの合計使用時間に制限をかける | Toggle | False |
| Admin Usable Time | 制限がある場合、Adminの利用可能時間 | Time | 120s |
| Dead can see everyone's roles | 死亡後に全員の役職がわかる | Toggle | True |
| Role Appears Under Name | 自身の役職を名前の下に表示する | Toggle | True |

-----------------------

# Bug / Suggestions

バグ報告、提案は [Twitter](https://twitter.com/Koke1024) までお願いします。

-----------------------

# Credits & Resources

[Town of Us](https://github.com/polusgg/Town-Of-Us) - The base of this MOD.\
[Reactor](https://github.com/NuclearPowered/Reactor) - The framework of the mod\
[BepInEx](https://github.com/BepInEx) - For hooking game functions\
[Among-Us-Sheriff-Mod](https://github.com/Woodi-dev/Among-Us-Sheriff-Mod) - For the Sheriff role.\
[ExtraRolesAmongUs](https://github.com/NotHunter101/ExtraRolesAmongUs) - For the Engineer & Medic roles.\
[TooManyRolesMods](https://github.com/Hardel-DW/TooManyRolesMods) - For the Investigator & Time Lord roles.\
[TorchMod](https://github.com/tomozbot/TorchMod) - For the inspiration of the Torch Mod.\
[XtraCube](https://github.com/XtraCube) - For the RainbowMod.\
[PhasmoFireGod](https://twitch.tv/PhasmoFireGod) - Button Art.\
[TheOtherRoles](https://github.com/Eisbison/TheOtherRoles) - For the inspiration of the Child and SecurityGuard roles.

[Essentials](https://github.com/DorCoMaNdO/Reactor-Essentials) - For created custom game options.

#

<p align="center">This mod is not affiliated with Among Us or Innersloth LLC, and the content contained therein is not endorsed or otherwise sponsored by Innersloth LLC. Portions of the materials contained herein are property of Innersloth LLC.</p>
<p align="center">© Innersloth LLC.</p>
