import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    categoryId: '88232da0-dd7a-11ec-a719-0242ac170002',
    customId: '1',
    tagCategory:[],
    customTagCategory:[]
  },
  mutations: {
    setCategoryId(state,id){
      state.categoryId = id;
    },
    setCustom(state, customId){
      state.customId = customId;
    },
    setTagCategory(state, tagCategory){
      state.tagCategory = tagCategory;
    },
    setCustomTagCategory(state, customTagCategory){
      state.customTagCategory = customTagCategory;
    }
  },
  actions: {
    updateCategoryId(context, id){
      context.commit('setCategoryId',id)
    },
    updateCustom(context, customId){
      context.commit('setCustom',customId)
    },
    updateTagCategory(context, tagCategory){
      context.commit('setTagCategory',tagCategory)
    },
    updateCustomTagCategory(context, customTagCategory){
      context.commit('setCustomTagCategory', customTagCategory)
    }
  },
  modules: {
  }
})
