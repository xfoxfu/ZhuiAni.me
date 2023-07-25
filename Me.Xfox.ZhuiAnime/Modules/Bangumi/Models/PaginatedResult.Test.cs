using System.Text.Json;

namespace Me.Xfox.ZhuiAnime.Modules.Bangumi.Models;

public class PaginatedResultTest
{
    [Test]
    public void TestDeserialize()
    {
        var result = JsonSerializer.Deserialize<PaginatedResult<Episode>>("""
{
    "data": [
        {
            "airdate": "2021-07-03",
            "name": "なにもかもダメになって",
            "name_cn": "一事无成",
            "duration": "00:50:40",
            "desc": "勤務するゲーム会社が立ち行かなくなり、実家へ帰省した橋場恭也。10年前に選択を間違わなければ……。後悔だらけの恭也がかつて受験した芸大の合格通知を眺めていると、なんと10年前にタイムスリップしてしまい……!?",
            "ep": 1,
            "sort": 1,
            "id": 1039546,
            "subject_id": 296870,
            "comment": 152,
            "type": 0,
            "disc": 0,
            "duration_seconds": 3040
        },
        {
            "airdate": "2021-07-10",
            "name": "10年前に戻ってきて",
            "name_cn": "回到十年前",
            "duration": "00:23:40",
            "desc": "授業で「時間」をテーマにした3分間の映像課題を作ることになった。\r\n恭也は、シェアハウスきたやまの3人とチームを組み、制作を担当することに。\r\nだが、恭也の提案したアイデアは貫之にある疑念を抱かせてしまう。",
            "ep": 2,
            "sort": 2,
            "id": 1039547,
            "subject_id": 296870,
            "comment": 91,
            "type": 0,
            "disc": 0,
            "duration_seconds": 1420
        },
        {
            "airdate": "2021-07-17",
            "name": "ばくは何者なんだろうって",
            "name_cn": "我是谁",
            "duration": "00:23:40",
            "desc": "ついに上映会当日。\r\n先に上映された河瀬川チームの作品は、恭也も圧倒されるほどのクオリティだった。\r\n万雷の拍手のあとに続く、恭也たちの作品。トラブル続きの絶望的な状況から生まれた作品とは……。",
            "ep": 3,
            "sort": 3,
            "id": 1039548,
            "subject_id": 296870,
            "comment": 73,
            "type": 0,
            "disc": 0,
            "duration_seconds": 1420
        },
        {
            "airdate": "2021-07-24",
            "name": "できることを考えて",
            "name_cn": "思考自己能做到的",
            "duration": "00:23:40",
            "desc": "河瀬川が加わったチームきたやま△は、新作映像の撮影のために海に来ていた。\r\n女性陣の水着姿にうろたえながらも、恭也は順調に撮影を進めるのだが……。\r\n上映会当日、思いがけない結果にナナコが複雑な表情を浮かべる。",
            "ep": 4,
            "sort": 4,
            "id": 1039549,
            "subject_id": 296870,
            "comment": 75,
            "type": 0,
            "disc": 0,
            "duration_seconds": 1420
        },
        {
            "airdate": "2021-07-31",
            "name": "自分の思いを打ちあけて",
            "name_cn": "表露真心",
            "duration": "00:23:40",
            "desc": "歌に本気で取り組むようになったナナコは、背中を押してくれる恭也の熱意に感激し、ふいに抱きついてしまう。\r\nそして、いよいよ学祭が開幕。\r\nすべてが順調に見えたが、最終日にとんでもないハプニングが待っていた。",
            "ep": 5,
            "sort": 5,
            "id": 1039550,
            "subject_id": 296870,
            "comment": 154,
            "type": 0,
            "disc": 0,
            "duration_seconds": 1420
        },
        {
            "airdate": "2021-08-07",
            "name": "なんとかしようって",
            "name_cn": "想办法解决",
            "duration": "00:23:40",
            "desc": "距離が近づいたシノアキに、どこかよそよそしいナナコ。\r\n学祭以来、恭也の周辺が色めき立つ中、貫之が過労で倒れてしまう。\r\n家庭の事情で学費を稼がないといけない貫之のために、恭也は同人ゲームの制作を提案する。",
            "ep": 6,
            "sort": 6,
            "id": 1039551,
            "subject_id": 296870,
            "comment": 81,
            "type": 0,
            "disc": 0,
            "duration_seconds": 1420
        },
        {
            "airdate": "2021-08-14",
            "name": "総集編",
            "name_cn": "总集篇",
            "duration": "00:23:40",
            "desc": "",
            "ep": 0,
            "sort": 6.5,
            "id": 1047618,
            "subject_id": 296870,
            "comment": 11,
            "type": 1,
            "disc": 0,
            "duration_seconds": 1420
        },
        {
            "airdate": "2021-08-21",
            "name": "いやなことも引きうけて",
            "name_cn": "讨厌的事情也承担",
            "duration": "00:23:40",
            "desc": "シェアハウスに貫之の婚約者さゆりが通ってくるようになった。\r\n貫之は困惑しつつも、シナリオ執筆に集中。\r\n一方、恭也はナナコから強烈なアプローチを受け、さらにシノアキからはきわどいスチルのモデルをさせられ……。",
            "ep": 7,
            "sort": 7,
            "id": 1039552,
            "subject_id": 296870,
            "comment": 74,
            "type": 0,
            "disc": 0,
            "duration_seconds": 1420
        },
        {
            "airdate": "2021-08-28",
            "name": "『結果』を出して",
            "name_cn": "给出结果",
            "duration": "00:23:40",
            "desc": "ゲーム制作が佳境を迎える中、恭也は各セクションに大胆な修正をお願いする。\r\n完成させるための最良の手段はそれしかない。\r\n納期を守りたいディレクターとオリジナリティを追求したいクリエイターの想いが交錯する。",
            "ep": 8,
            "sort": 8,
            "id": 1039553,
            "subject_id": 296870,
            "comment": 100,
            "type": 0,
            "disc": 0,
            "duration_seconds": 1420
        },
        {
            "airdate": "2021-09-04",
            "name": "見せつけられて",
            "name_cn": "见识到",
            "duration": "00:23:40",
            "desc": "目を覚ますと2018年の世界だった。30歳の恭也はなぜか幸せな家庭を築いていたが、ここまでの過程がわからない。\r\nこの世界でも同僚になっていた河瀬川を頼りに、仲間たち「プラチナ世代」の行方を探るが……。",
            "ep": 9,
            "sort": 9,
            "id": 1039554,
            "subject_id": 296870,
            "comment": 50,
            "type": 0,
            "disc": 0,
            "duration_seconds": 1420
        },
        {
            "airdate": "2021-09-11",
            "name": "思い知らされて",
            "name_cn": "认识到",
            "duration": "00:23:40",
            "desc": "温かい日常はあるが、キラキラ輝くプラチナ世代はもういない。\r\n恭也は二度と大きな間違いを繰り返さないよう、ひたすら仕事に打ち込んでいた。\r\nそんな中、河瀬川チームの開発が遅れ、彼女が窮地に立たされてしまう。",
            "ep": 10,
            "sort": 10,
            "id": 1039555,
            "subject_id": 296870,
            "comment": 79,
            "type": 0,
            "disc": 0,
            "duration_seconds": 1420
        },
        {
            "airdate": "2021-09-18",
            "name": "覚悟を決めて",
            "name_cn": "下定决心",
            "duration": "00:23:40",
            "desc": "ナナコの力強い言葉に背中を押された恭也は、評判が地に落ちた「ミスクロ」を再生させるために、社長と対峙し強硬な手段をとろうとする。\r\nそれは、これまでのやり方を根底から覆す、あまりに型破りな方法だった。",
            "ep": 11,
            "sort": 11,
            "id": 1039556,
            "subject_id": 296870,
            "comment": 85,
            "type": 0,
            "disc": 0,
            "duration_seconds": 1420
        },
        {
            "airdate": "2021-09-25",
            "name": "もういちど前を向いた",
            "name_cn": "再一次向前看",
            "duration": "00:23:40",
            "desc": "恭也は、再び選択を迫られる。\r\n妻も子どももいる幸せなこの世界か、つらくて、痛くて、未来も見えないあの世界か。\r\nそれでも、あの世界に戻りたいと願う。仲間とともに「作りたいもの」を追い続けたあの世界に……。",
            "ep": 12,
            "sort": 12,
            "id": 1039557,
            "subject_id": 296870,
            "comment": 45,
            "type": 0,
            "disc": 0,
            "duration_seconds": 1420
        }
    ],
    "total": 13,
    "limit": 100,
    "offset": 0
}
""")!;
        result.Data.Count().Should().Be(13);
        result.Total.Should().Be(13);
        result.Limit.Should().Be(100);
        result.Offset.Should().Be(0);
    }
}
