# game-ai-behaviour
![image](https://user-images.githubusercontent.com/6957962/212475238-3cf6ddfe-6304-4c3b-9b17-27be8459fcc9.png)
## 概要
#### 特徴
* MonoBehaviourに依存しないBehaviourTreeツール
* データ構造と実行処理を分離しやすくして、依存関係を切り離しやすいクラス設計
* シンプルな機能や拡張設計
#### 背景
既存のBehaviourTreeは、AIを取り巻く環境としてヘビーな物が多く、BehaviourTreeをワンポイントで使いたいなと思いさ作成したツールです。
## セットアップ
#### インストール
1. Window > Package ManagerからPackage Managerを開く
2. 「+」ボタン > Add package from git URL
3. 以下を入力してインストール
   * https://github.com/DaitokuAmy/game-ai-behaviour.git?path=/Assets/GameAiBehaviour
   ![image](https://user-images.githubusercontent.com/6957962/209446846-c9b35922-d8cb-4ba3-961b-52a81515c808.png)

あるいはPackages/manifest.jsonを開き、dependenciesブロックに以下を追記します。
```json
{
    "dependencies": {
        "com.daitokuamy.gamaibehaviour": "https://github.com/DaitokuAmy/game-ai-behaviour.git?path=/Assets/GameAiBehaviour"
    }
}
```
バージョンを指定したい場合には以下のように記述します。  
https://github.com/DaitokuAmygame-ai-behaviour.git?path=/Assets/GameAiBehaviour#1.0.0
