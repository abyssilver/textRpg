namespace textRPG
{
    internal class Program
    {
        static Player player;

        static void Main(string[] args)
        {


            // 저장된 player 정보가 있는지 확인하고 없으면 startScene() 실행
            //일단 없다고 가정
            StartScene();

            //마을 행동 턴
            TownPhase();







        }
        static void StartScene()
        {
            Console.WriteLine("=== 스파르타 RPG에 오신 것을 환영합니다! ===");

            while (true)
            {
                Console.WriteLine("플레이어 이름을 입력하세요:");
                string name = Console.ReadLine();

                Console.WriteLine($"\n입력하신 이름은 \"{name}\" 입니다. 해당 이름으로 시작하겠습니까?");
                Console.WriteLine("1. 시작한다\n2. 다시 정한다");
                string input = Console.ReadLine();

                if (input == "1")
                {
                    player = new Player(name);
                    Console.WriteLine($"플레이어 {name}으로 시작합니다.");
                    break;
                }
                else if (input == "2") { continue; }
                else
                {
                    Console.WriteLine("잘못된 입력입니다. 다시 진행해주세요.");
                    continue;
                }
            }
        }

        static void TownPhase()
        {
            while (true)
            {

                Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
                Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
                Console.WriteLine("\n1. 상태 보기\n2. 인벤토리\n3. 상점\n4. 던전 입장\n5. 휴식하기\n");
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                string input = Console.ReadLine();
                if (input == "1")
                {
                    //상태창
                    player.PrintInfo();
                    input = Console.ReadLine();
                    if (input == "0")
                    {
                        continue;
                    }

                }
                else if (input == "2")
                {
                    player.PrintInventory();

                }
                else if (input == "3")
                {
                    Shop();
                }
                else if (input == "4")
                {
                    DungeonPhase();
                }
                else if (input == "5")
                {
                    Rest();
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다");
                }
            }
        }

        static void Shop()
        {
            while (true)
            {
                Console.WriteLine("상점");
                Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
                Console.WriteLine($"\n[보유 골드] {player.Gold} G\n");

                Console.WriteLine("[아이템 목록]");
                foreach (Item item in ItemDB.Items)
                {
                    Console.WriteLine(item); // 숫자 없이 출력
                }

                Console.WriteLine("\n1. 아이템 구매\n2. 아이템 판매\n0. 나가기");
                Console.Write("원하시는 행동을 입력해주세요: ");
                string input = Console.ReadLine();

                if (input == "1")
                {
                    while (true)
                    {
                        Console.WriteLine($"\n[보유 골드] {player.Gold} G\n");
                        Console.WriteLine("\n[아이템 구매 목록]");
                        for (int i = 0; i < ItemDB.Items.Count; i++)
                        {
                            Item item = ItemDB.Items[i];
                            bool alreadyOwned = player.inventory.Any(invItem => invItem.Name == item.Name);

                            if (alreadyOwned)
                            {
                                Console.WriteLine($"{i + 1}. {item.Name} | {item.Description} | 구매완료");
                            }
                            else
                            {
                                Console.WriteLine($"{i + 1}. {item}");
                            }
                        }

                        Console.WriteLine("0. 나가기");
                        Console.Write("구매할 아이템 번호를 입력하세요: ");
                        string selectedInput = Console.ReadLine();

                        if (int.TryParse(selectedInput, out int selected))
                        {
                            if (selected == 0)
                            {
                                break;
                            }

                            if (selected > 0 && selected <= ItemDB.Items.Count)
                            {
                                Item item = ItemDB.Items[selected - 1];
                                bool alreadyOwned = player.inventory.Any(invItem => invItem.Name == item.Name);

                                if (alreadyOwned)
                                {
                                    Console.WriteLine($"이미 구매한 아이템입니다: {item.Name}");
                                }
                                else if (player.Gold >= item.Price)
                                {
                                    player.Gold -= item.Price;
                                    player.inventory.Add(item);
                                    Console.WriteLine($"{item.Name}을 구매했습니다!");
                                }
                                else
                                {
                                    Console.WriteLine("골드가 부족합니다.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("해당 번호의 아이템은 존재하지 않습니다.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("숫자를 입력해주세요.");
                        }
                    }
                }
                else if (input == "2")
                {
                    if (player.inventory.Count == 0)
                    {
                        Console.WriteLine("판매할 아이템이 없습니다");
                    }
                    else
                    {
                        while (true)
                        {
                            Console.WriteLine("\n[아이템 목록]\n");

                            for (int i = 0; i < player.inventory.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {player.inventory[i]}");

                            }
                            Console.WriteLine("0. 나가기");
                            Console.Write("판매할 아이템을 선택해주세요: ");
                            string selectedInput = Console.ReadLine();
                            if (int.TryParse(selectedInput, out int selected))
                            {
                                if (selected == 0)
                                {
                                    break;
                                }
                                else if (selected > 0 && selected <= player.inventory.Count)
                                {
                                    Item selectedItem = player.inventory[selected - 1];
                                    Item weapon = player.Slots[(int)EquipSlot.Weapon];
                                    Item clothes = player.Slots[(int)EquipSlot.Clothes];
                                    bool isEquipped = selectedItem == weapon || selectedItem == clothes;
                                    if (isEquipped)
                                    {
                                        Console.WriteLine("현재 장착중인 아이템입니다. 장착해제 후 판매해주세요");
                                    }
                                    else
                                    {
                                        int sellPrice = (int)(selectedItem.Price * 0.85f);
                                        player.Gold += sellPrice;
                                        player.inventory.RemoveAt(selected - 1);
                                        Console.WriteLine($"{selectedItem.Name}을(를) 판매했습니다. ({sellPrice}G 획득)");
                                    }

                                }
                                else
                                {
                                    Console.WriteLine("해당 번호의 아이템은 존재하지 않습니다.");
                                }

                            }
                            else
                            {
                                Console.WriteLine("숫자를 입력해주세요.");
                            }
                        }
                    }
                }
                else if (input == "0")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("잘못 입력하셨습니다.");
                }
            }
        }

        static void Rest()
        {
            int missingHp = player.MaxHp - player.Hp;
            int cost = missingHp * 3;
            if (player.Gold < cost)
            {
                Console.WriteLine("골드가 부족합니다.");
                return;
            }

            if (missingHp == 0)
            {
                Console.WriteLine("이미 체력이 가득 찼습니다.");
                return;
            }

            Console.WriteLine($"휴식을 하려면 {cost}G가 필요합니다.");

            Console.WriteLine("휴식하시겠습니까?\n1. 휴식한다\n0. 하지않는다.");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int selected))
            {
                switch (selected)
                {
                    case 0: return;
                    case 1:
                        player.Gold -= cost;
                        player.Hp = player.MaxHp;
                        Console.WriteLine($"휴식을 완료했습니다. 체력이 {missingHp}만큼 회복되었습니다. ({cost}G 소모)");
                        break;
                    default:
                        Console.WriteLine("1이나 0을 입력해주세요.");
                        break;

                }
            }
            else
            {
                Console.WriteLine("숫자를 입력해주세요.");
            }


        }
        static void DungeonPhase()
        {
            //던전ui 작성
            if (player.Hp == 0)
            {
                Console.WriteLine("체력이 없어 입장할수 없습니다.");
                return;
            }
            Console.WriteLine("던전입장\n이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            for (int i = 0; i < DungeonBD.Dungeons.Count; i++)
            {
                var d = DungeonBD.Dungeons[i];
                Console.WriteLine($"{i + 1}. {d.Name} | 권장 방어력: {d.RecommendDef} | 기본 보상: {d.Reward}G");
            }
            Console.WriteLine("0. 나가기");

            Console.WriteLine("원하시는 행동을 입력해주세요.");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int selected))
            {
                if (selected == 0) return;
                if (selected >= 1 && selected <= DungeonBD.Dungeons.Count)
                {
                    Dungeon selectedDungeon = DungeonBD.Dungeons[selected - 1];
                    DungeonSystem(selectedDungeon);

                }
                else
                {
                    Console.WriteLine("올바른 번호를 입력해주세요.");
                }
            }
            else
            {
                Console.WriteLine("숫자를 입력해주세요.");
            }
            

        }
        static void DungeonSystem(Dungeon dungeon)
        {

            int totalDef = player.Def;
            if (player.Slots[(int)EquipSlot.Clothes] is Clothes clothes)
            {
                totalDef += clothes.Def;
            }
            int totalAtk = player.Atk;
            if (player.Slots[(int)EquipSlot.Weapon] is Weapon weapon)
            {
                totalAtk += weapon.Atk;
            }

            Random rand = new Random();
            if (totalDef < dungeon.RecommendDef)
            {
                //40퍼 확률로 던전 실패
                bool isFailed = rand.Next(100) < 40;
                player.Hp -= player.MaxHp / 2;
                if (player.Hp < 0) player.Hp = 0;
                Console.WriteLine($"던전 클리어 실패\n\n{dungeon.Name}을 클리어 실패 하였습니다.");
                Console.WriteLine($"[탐험 결과]\n남은 체력: {player.Hp}\n\n 나가려면 아무 버튼이나 눌러주세요.");
                Console.ReadLine();


                return;
            }

            else
            {
                //던전 클리어
                /*기본으로 20~35 랜덤 체력감소 , totalDef - dungeon.RecommendDef 만큼 체력 감소 경감*/

                int baseDamage = rand.Next(20, 36);
                int reward = dungeon.Reward + rand.Next(totalAtk, totalAtk * 2);

                int mitigated = Math.Max(0, baseDamage - (totalDef - dungeon.RecommendDef));
                player.Hp -= mitigated;
                if (player.Hp < 0) player.Hp = 0;
                Console.WriteLine($"던전 클리어\n축하합니다!!\n{dungeon.Name}을 클리어 선공 하였습니다.");
                Console.WriteLine($"[탐험 결과]\n남은 체력: {player.Hp}\n 보상 : {reward} G\n 나가려면 아무 버튼이나 눌러주세요.");
                player.Gold += reward;
                player.DungeonClearCount++;

                if (player.DungeonClearCount >= player.Level)
                {
                    player.Level++;
                    player.Atk += 5;
                    player.Def += 5;
                    player.DungeonClearCount = 0;
                    Console.WriteLine($"레벨업! Lv.{player.Level}이 되었습니다! 공격력과 방어력이 각각 5 상승합니다.");
                }
                Console.ReadLine();

                return;



            }

        }

        public class Player
        {
            public string Name;
            public int Level;
            public string Job;
            public int Atk;
            public int Def;
            public int Hp;
            public int MaxHp;
            public int Gold;
            public int DungeonClearCount;


            public List<Item> inventory = new List<Item>();
            public Item[] Slots = new Item[2];

            public Player(string name)
            {
                Name = name;
                Level = 1;
                Job = "전사";
                Atk = 10;
                Def = 5;
                Hp = 100;
                MaxHp = 100;
                Gold = 1500;
            }

            public void PrintInfo()
            {
                int equipAtk = 0;
                int equipDef = 0;
                Console.WriteLine("상태 보기");
                Console.WriteLine("캐릭터의 정보가 표시됩니다.");
                Console.WriteLine("\n");

                if (Slots[(int)EquipSlot.Weapon] is Weapon weapon)
                {
                    equipAtk = weapon.Atk;
                }
                if (Slots[(int)EquipSlot.Clothes] is Clothes clothes)
                {
                    equipDef = clothes.Def;
                }
                Console.WriteLine($"Lv. {Level}");
                Console.WriteLine($"{Name} ( {Job} )");
                Console.WriteLine($"공격력 : {Atk + equipAtk}");
                Console.WriteLine($"방어력 : {Def + equipDef}");
                Console.WriteLine($"체력 : {Hp}/{MaxHp}");
                Console.WriteLine($"Gold : {Gold}G");
                Console.WriteLine("\n");
                Console.WriteLine("0. 나가기");
            }
            public void PrintInventory()
            {
                while (true)
                {
                    Console.WriteLine("인벤토리");
                    Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
                    Console.WriteLine("\n[아이템 목록]\n");

                    foreach (Item item in inventory)
                    {
                        Console.WriteLine(item);

                    }
                    Console.WriteLine("\n1. 장착 관리\n0. 나가기\n");
                    Console.WriteLine("원하시는 행동을 입력해주세요.");
                    string input = Console.ReadLine();
                    if (input == "1")
                    {
                        if (inventory.Count == 0)
                        {
                            Console.WriteLine("관리할 아이템이 없습니다");
                        }
                        else
                        {
                            // 장착 관리 기능
                            while (true)
                            {
                                Console.WriteLine("\n[장착 관리 가능한 아이템 목록]");
                                List<IEquipable> equipables = new List<IEquipable>();

                                for (int i = 0; i < inventory.Count; i++)
                                {
                                    if (inventory[i] is IEquipable eq)
                                    {
                                        equipables.Add(eq);
                                    }
                                }

                                if (equipables.Count == 0)
                                {
                                    Console.WriteLine("장착 가능한 아이템이 없습니다.");
                                    break;
                                }

                                for (int i = 0; i < equipables.Count; i++)
                                {
                                    var item = (Item)equipables[i];
                                    var slotType = equipables[i].SlotType;
                                    bool isEquipped = Slots[(int)slotType] == item;

                                    string prefix = isEquipped ? "[E] " : "";
                                    Console.WriteLine($"{i + 1}. {prefix}{item}");
                                }

                                Console.WriteLine("0. 나가기");
                                Console.Write("장착하거나 해제할 아이템 번호를 입력하세요: ");
                                string sel = Console.ReadLine();

                                if (int.TryParse(sel, out int selected))
                                {
                                    if (selected == 0) break;

                                    if (selected > 0 && selected <= equipables.Count)
                                    {
                                        IEquipable selectedItem = equipables[selected - 1];
                                        var slotIndex = (int)selectedItem.SlotType;

                                        if (Slots[slotIndex] == selectedItem)
                                        {
                                            // 해제
                                            Slots[slotIndex] = null;
                                            Console.WriteLine($"{((Item)selectedItem).Name}을(를) 해제했습니다.");
                                        }
                                        else
                                        {
                                            // 장착
                                            selectedItem.Equip(this);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("해당 번호의 아이템은 존재하지 않습니다.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("숫자를 입력해주세요.");
                                }
                            }

                        }
                    }
                    else if (input == "0")
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("잘못 입력하셨습니다.");
                    }
                }

            }

        }


        public abstract class Item
        {
            public string Name { get; protected set; }
            public int Price { get; protected set; }
            public string Description { get; protected set; }

            protected Item(string name, int price, string description)
            {
                Name = name;
                Price = price;
                Description = description;
            }


        }
        public interface IEquipable
        {
            EquipSlot SlotType { get; }
            void Equip(Player player);

        }
        public enum EquipSlot
        {
            Weapon,
            Clothes
        }
        class Weapon : Item, IEquipable
        {
            public int Atk { get; private set; }

            public Weapon(string name, int price, string description, int atk)
                : base(name, price, description)
            {
                Atk = atk;
            }
            public EquipSlot SlotType => EquipSlot.Weapon;

            public void Equip(Player player)
            {
                player.Slots[(int)SlotType] = this;
                Console.WriteLine($"{Name}을 장착했습니다. (공격력 +{Atk})");
            }
            public override string ToString()
            {
                return $"{Name} | 공격력 +{Atk} | {Description} | {Price}G";
            }




        }

        class Clothes : Item, IEquipable
        {
            public int Def { get; private set; }

            public Clothes(string name, int price, string description, int def)
                : base(name, price, description)
            {
                Def = def;
            }
            public EquipSlot SlotType => EquipSlot.Clothes;

            public void Equip(Player player)
            {
                player.Slots[(int)SlotType] = this;
                Console.WriteLine($"{Name}을 장착했습니다. (방어력 +{Def})");
            }
            public override string ToString()
            {
                return $"{Name} | 방어력 +{Def} | {Description} | {Price}G";
            }

        }

        static class ItemDB
        {
            public static List<Item> Items = new List<Item>()
            {
            new Weapon("철검", 100, "단순한 철검이다.", 10),
            new Weapon("은검", 200, "은으로 제작된 검이다.", 20),
            new Clothes("낡은 옷", 50, "낡은 방어구입니다.", 5),
            new Clothes("가죽 갑옷", 150, "가죽으로 만들어진 방어구입니다.", 10)

            };
        }
        public class Dungeon
        {
            public string Name;
            public int RecommendDef;
            public int Reward;

            public Dungeon(string name, int recommendDef, int reward)
            {
                Name = name;
                RecommendDef = recommendDef;
                Reward = reward;
            }
        }
        static class DungeonBD
        {
            public static List<Dungeon> Dungeons = new List<Dungeon>()
            {
                new Dungeon("초급 던전", 5, 1000),
                new Dungeon("중급 던전", 11, 1700),
                new Dungeon("상급 던전", 17, 2500)
            };
        }
    }
}


