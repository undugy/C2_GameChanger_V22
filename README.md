# C2_GameChanger_V22

------
## 목차
1. 구성
2. 소스코드 설명
----
## 01. 구성

```
- .NET Core 6.0

- 데이터베이스
  * MySQL 8.0
  * Redis

- NuGet
  * Dapper
  * ZLogger
  * CloudStructures
```

- Dapper
```
  - OOP에서 DB를 쉽게 사용하기 위한 ORM 도구
  - ORM : DB에서 받은 데이터 타입을 객체지향언어에서 이해할 수 있는 데이터 타입으로 변환해 주는 것이 ORM의 역할
  - 장점
    1. 개발이 간단하고 속도가 빠르다.
    2. 많은 Extension 존재
    3. ADO .NET을 기반으로 Data to Strong typed Entity로 전환만 제공하므로 가볍다.
    4. 기본적으로 쿼리 결과를 버퍼에 담아서 반환하기 때문에 대부분의 경우 db에 락을 적게 발생 시킨다.
  - 단점
    1. 직접 쿼리를 짜야하므로 도메인 구조가 바뀌면 쿼리를 같이 바꿔줘야 하는 경우가 있다.
    2. 하나의 Row가 하나의 Entity에 매핑되므로 Nested Entity등의 복잡한 구조는 추가적인 작업을 해줘야 한다.
```
- Cloud Structures
```
* StaExchange.Redis를 사용하여 CloudStructures는 Redis에 기능을 추가하고 사용하기 편하게 하는 라이브러리

* Redis는 Key Value이기 때문에 Key에 Value 하나만 넣을 수 있다. 그래서 Class Data를 넣을때 Binary로 변환해서 넣어주어야 한다.

* 하지만 CloudStructures에서는 Utf8Json을 default로 제공하고 있어 위의 과정을 거치지 않아도 된다.

* 하지만 Utf8Json을 사용해 Json파일로 변환하게 되면 Binary 파일보다 데이터가 커지게 되는데 이럴 때는 MessagePack을 사용하여 크기를 줄이고 인코딩, 디코딩 속도를 좀 더 빠르게 할 수 있다.
```

- ZLogger
```
* 일반적인 Logger,WriteLine 방식은 object를 boxing하고 string을 UTF8로 인코딩하는데 추가비용이 들었다. 

* ZLogger는 제로할당 문자열 빌더 ZString에 의해 UTF8로 직접 버퍼영역에 쓰게되고 ConsoleStream에 정리해서 보내주기 때문에  boxing도 발생하지 않고 비동기적으로 단번에 쓰기 때문에 애플리케이션에 부하를 주지 않는다.
```
## 02. 소스코드 분석

### 1. Program.cs

- GameDatabase와 MasterDatabase를 Transient로 종속성 주입

- Redis는 Singleton으로 -> 기획데이터 불러오기 때문

- ZLogger 추가

- DB 초기화 및 미들웨어 추가
-----
### 2. Controllers

- CheckInController.cs
  ``` 
  - 출석체크를 위한 컨트롤러
  - gameDatabase와 Redis, logger 종속성 주입

  - Post 함수
    1. request로 콘텐츠 타입을 받는다.
    2. gameDatabase 에서 user의 출석정보와 마지막 접속시간을 받는다.
    3. 만약 마지막 접속과 현재시간이 1일이 안지났고 출석체크를 안했다면 redis로 부터 출석 아이템을 가져오고 받은 아이템을 user inventory에 넣어준다.
    4. 받은 날짜를 오늘로 해주고 CheckDay에 하루를 더해준다.
    5. gameDatabase에 isChecked와 Check Day를 업데이트 해준다.
  ```

- CreateAccountController.cs
  ```SQL
  - 회원가입을 위한 컨트롤러
  - gameDatabase, logger 종속성 주입

  - Post함수
    1. request로 아이디와 패스워드를 받는다.
    2. saltvalue 생성 및 해싱 패스워드를 sha 256을 이용해 salt와 비밀번호를 조합해 만든다.
    3. INSERT  INTO user_info(Email,HashedPassword,SaltValue) 
    SELECT @email,@pw,@salt 
    FROM DUAL 
    WHERE NOT EXISTS(SELECT * FROM user_info WHERE Email=@email)
    쿼리를 이용해 중복체크하고 넣어준다.

  ```

- LoginController.cs
  ```
  - 로그인 위한 컨트롤러
  - gameDatabase와 Redis, logger 종속성 주입

  - Post함수
    1. request로 부터 ID, PW를 받는다.
    2. DB에서 유저의 정보를 가져온다.
    3. DB에서 가져온 salt와 request에서 받은 PW를 조합해 해싱 패스워드를 만들고 그걸 다시 대조해본다.
    4. 맞다면 토큰을 발급하고 response에 보내준다.
  ```

- MailController
  ```
   - 매일을 위한 컨트롤러
  - gameDatabase와 Redis, logger 종속성 주입

  - ListUp Post함수
    1. MailListRequest를 리퀘스트를 받아
    2. DB에서 Mail리스트를 전부 불러와 보내준다.
  
  - Post 함수
    1. 메일 하나를 수령하는 기능
    2. MailRequest를 받아 DB에서 해당 메일을 불러온다.
    3. item이 재화인지 아이템 인지에 따라 DB에 넣어준다.
    4. 메일을 삭제한다.
    5. 수령한 메일정보를 response로 보내준다.

  - ReceiveAll Post 함수
    - 그냥 Post함수와 비슷하지만 메일을 단체로 처리하는 차이
  ```

- SetUpUserDataController
  ```
  - 유저의 정보를 불러오기 위한 컨트롤러
  - gameDatabase, masterDatabase, redis, logger 종속성 주입

  - Post 함수
    1. lastAccess와 userAttendance를 가져온다 만약 최초 접속이라면 클라에 팀선택으로 넘기기위해 NOT_INIT 리스폰스 보내준다.

    2.만약 접속한 지금이 마지막 접속으로 부터 1일이 지났고 IsChecked가 false라면 전날 접속했었지만 출석체크를 하지 않은 것이기 때문에 이전 날짜의 보상을 지급한다.

    3. DB에 mail, UserAttendance, UserAccess 정보를 업데이트 해준다.

  - InitializeTeam Post함수
    1. 이름변경권 지급
    2. request로 부터 받은 team name을 이용해 기본닉네임을 만들어준다.
    3. UserTeam,UserItem,UserAttendance,UserAccess를 DB에 업데이트 해준다.
  ```
----
### 3. Interface
- IDataBase : 기본 DB 인터페이스 커넥션 함수
- 종속성 주입을 위한 interface
  - IGameDataBase
  - IMasterDataBase
  - IRedisDataBase
----
### 4. Middleware
- MiddlewareExtensions : 미들웨어 등록을 위한 static class

- CheckUserSessionMiddleWare
  - 중복 로그인을 방지하기 위한 토큰확인 미들웨어
  - context에서 아이디와 토큰을 읽어 redis에서 확인하고 없으면 return 
  - 있다면 다음으로 넘어간다.

### 5. Model
- ReqRes
  - 리퀘스트와 리스폰스를 위한 모델

- User
  - ORM을 위한 모델
  - 각각 Insert, Update쿼리를 가지고 있음
  - UserAccess
    - 접속을 관리하기 위한 테이블 
    - 생성날짜, 마지막 접속날짜, id를 가짐
  

  - UserAttendance
    - 출석체크 테이블
    - 출석체크 종류, 체크한 총날짜, 당일체크했는지 확인
  
  - UserInfo
    - 유저정보 테이블
    - id, email, saltvalue, hashedpassword 관리
    ```SQL
    InsertQuery 
    INSERT  INTO user_info(Email,HashedPassword,SaltValue)
    SELECT @email,@pw,@salt FROM DUAL WHERE NOT EXISTS(SELECT * FROM user_info WHERE Email=@email)
    ```
  - UserItem
    - 아이템 테이블
    - DB 테이블 이름은 user_bag
    - 아이템 아이디, 소유하고 있는 유저 아이디, 개수, 종류 관리
    ```SQL
    InsertQuery 
    INSERT INTO user_bag(UserId,ItemId,  Quantity,Kind)
    VALUES (@itemId,@userId,@quantity,@kind)
    ON DUPLICATE KEY UPDATE Quantity=Quantity+@quantity
    ```

  - UserMail
    - 메일 아이디, 유저 아이디, 메일의 종류, 받은 아이템 아이디/개수, 받은날짜 관리

  - UserTeam
    - 재화, 경험치, 닉네임, 팀 아이디, 레벨, 소개 관리
    ```sql
    UpdateWealthQuery
    UPDATE user_team SET wealthName = wealthName + @Quantity WHERE UserId=@userId
    ```
----
### 5. Service

#### 1. GameDataBase

- IGameDataBase를 상속

- GetOpenMySqlConnection
  - 커넥션을 할당하고 
  - OpenAsync 함수호출후
  - 커넥션 return

- SelectSingleUserInfo
  - QuerySingleOrDefaultAsync함수로 DB에서 userInfo를 가져온다.

- SelectUserLastAccess
  -  QuerySingleOrDefaultAsync함수로 DB에서 userAccess를 가져온다.

- SelectSingleUserAttendance
  - QuerySingleOrDefaultAsync함수로 DB에서 user_attendance를 가져온다

- UpdateUserLastAccess 
  - user_Log의 마지막 접속을 업데이트 한다.

- MakeSetUpResponse
  - user의 team, mail, attendance를 전부 불러오고
  - 리스트로 만들어 반환
  - QueryMultipleAsync 사용

- MakeCheckInResponse
  - 아이템 아이디에 따라 재화라면 team 테이블에 재화를 넣어주고, 아이템이라면 user_bag테이블에 넣어준다.

- SelectMail,GetMailList
  - 메일을 단일로 혹은 전체로 불러오는 함수

- DeleteMail, DeleteAllMail
  - 메일을 단일로 혹은 전체로 삭제하는 함수

- ReceiveByItemId, ReceiveByItemName
  - 메일로부터 아이템을 받는 함수

-----
#### 2. MasterDataBase

- SelectSingleItemId
- SelectSingleTeamId
- SelectSingleDailyCheckIn

- 마스터 데이터베이스에 있는 데이터를 QuerySingleOrDefaultAsync 하나씩 가져오는 함수로 구성
-----
#### 3. RedisDataBase
- 생성자에서 기획데이터를 마스터 데이터베이스에서 불러와 redis에 올린다

- GetHashValue : RedisDictionary에서 단일로 가져온다.

- GetHash : RedisDictionary 자체로 가져온다.

- GetStringValue, SetStringValue String 자료구조로 저장하거나 가져오는 함수

