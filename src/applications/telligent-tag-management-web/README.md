# telligent-tag-management-web

## Project setup
```
yarn install
```

### Compiles and hot-reloads for development
```
yarn serve:dev
```

### Compiles and minifies for production
```
yarn build
```

### Lints and fixes files
```
yarn lint
```

### Customize configuration
See [Configuration Reference](https://cli.vuejs.org/config/).
### Vue 版本 2.6.11
### api 檔案命名方式
ControllerName+'Api'
### api 命名方式 
API名稱+Async
### api 呼叫方式 
import API 檔案 ，引入需要使用的方法， 使用非同步方式呼叫 利用axios 套件結合 async/await
ex: await getbatchTransactionLogApi()，可參考 apis/batchTransactionLogApi
### api 呼叫路徑會依照 開發環境有所不同，故需設定相關環境變數，可參考 config.js   api: process.env.VUE_APP_TAG_API,  
### ui 套件 採用 Vuetify UI V2.6.6
### 多語系使用方法 $t('名稱')

### Vue 路徑設定 router.js
### Vue store 參考 store.js
