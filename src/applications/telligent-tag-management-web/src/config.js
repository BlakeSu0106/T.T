export const config = {
  api: process.env.VUE_APP_TAG_API,  
};

export default {
  install(Vue) {
    Vue.prototype.$conf = config;
  },
};
