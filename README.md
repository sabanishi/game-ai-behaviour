# game-ai-behaviour
![image](https://user-images.githubusercontent.com/6957962/212475238-3cf6ddfe-6304-4c3b-9b17-27be8459fcc9.png)
## 概要
#### 特徴
* MonoBehaviourに依存しないBehaviourTreeツール
* データ構造と実行処理を分離しやすくして、依存関係を切り離しやすいクラス設計
* シンプルな機能や拡張設計
#### 背景
既存のBehaviourTreeは、AIを取り巻く環境としてヘビーな物が多く、BehaviourTreeをワンポイントで使いたいなと思って作成したツールです。
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
## 機能
#### Root
* Root Node  
![root_node](https://user-images.githubusercontent.com/6957962/212528550-d828bee6-d619-4493-93d1-00a64157b9d0.jpg)

#### Composite
* Selector Node  
![selector_node](https://user-images.githubusercontent.com/6957962/212528596-fff170a3-109d-4cb5-b7a9-587e9b7a37c3.jpg)

* Sequencer Node  
![sequencer_node](https://user-images.githubusercontent.com/6957962/212528605-244271cd-c36a-4d17-a810-b73203eee5d8.jpg)

#### Decorator
* If Node  
![if_node](https://user-images.githubusercontent.com/6957962/212528619-c481ef71-2c6e-4e1f-98f5-e49ecfd52ef2.jpg)

* While Node  
![while_node](https://user-images.githubusercontent.com/6957962/212528622-a559864e-dbd1-4eee-a1c4-34834c03b45c.jpg)

* Repeat Node  
![repeat_node](https://user-images.githubusercontent.com/6957962/212528629-f5bca380-0584-4140-98bd-945bd059e4c5.jpg)

* Through Node  
![through_node](https://user-images.githubusercontent.com/6957962/212528639-834a5d1a-5026-41bd-a83e-dfbad9f04da6.jpg)

#### Action
* Log Node  
![log_node](https://user-images.githubusercontent.com/6957962/212528664-e759e37d-cda5-4243-9147-8827c328e508.jpg)

* Reset Node  
![reset_node](https://user-images.githubusercontent.com/6957962/212528674-aea1b816-b201-4f33-b8ec-90e93ee27004.jpg)

* Wait Node  
![wait_node](https://user-images.githubusercontent.com/6957962/212528678-de3b5dfe-fcde-4d49-9247-541388259c3c.jpg)

#### Function Root
* Function Root Node  
![function_root_node](https://user-images.githubusercontent.com/6957962/212528686-29b21c48-ef4a-46fd-aa0a-ce1c34cd6098.jpg)

#### Link
* Function Node  
![function_node](https://user-images.githubusercontent.com/6957962/212528700-c6faabfd-e505-4a69-a256-1750a12cd788.jpg)

* Sub Tree Node  
![sub_tree_node](https://user-images.githubusercontent.com/6957962/212528696-cd976a75-7dc6-4d3a-adbd-276336e44a11.jpg)
