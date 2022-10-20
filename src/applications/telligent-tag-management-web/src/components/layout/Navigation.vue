<template>
  <v-navigation-drawer app clipped color="#F8F9FA">
    <v-list-group
      v-for="item in items"
      :key="item.id"
      :value="true"
      @click="dropdownStatus = !dropdownStatus"
      :append-icon="dropdownStatus ? 'mdi-chevron-double-down' : 'mdi-chevron-double-down'"
    >
      <template v-slot:activator>
        <v-list-item-content>
          <v-list-item-title>{{item.title}}</v-list-item-title>
        </v-list-item-content>
      </template>

      <v-list-item-group v-model="selection">
        <v-list-item
          :value="child.id"
          v-for="child in item.child"
          :key="child.id"
          @click="clickAction(child.id,item.id,child);"
          dense
        >
          <v-list-item-title>{{child.name}}</v-list-item-title>
          <v-badge
            inline
            color="grey lighten-1"
            :content="child.tagQuantity===0?'0':child.tagQuantity"
          ></v-badge>
        </v-list-item>
      </v-list-item-group>
    </v-list-group>
  </v-navigation-drawer>
</template>

<script>
import { getBehaviorTagCategoryAsync } from "@/apis/behaviorTagCategoryApi";
import { getAllCustomizationTagCategoryAsync } from "@/apis/customizationTagCategoryApi";
import { filter } from "minimatch";

export default {
  data: function() {
    return {
      dropdownStatus: false,
      items: [
        {
          title: "系統行為標籤分類",
          id: "1",
          child: [],
          count: 0
        },
        {
          title: "自訂人工貼標分類",
          id: "2",
          child: [],
          count: 0
        }
      ],
      tagCategory: [],
      selection: "",
      selectedStatus: false
    };
  },
  methods: {
    async getBehaviorTagCategory() {
      let resp = await getBehaviorTagCategoryAsync(
        "33104d76-30be-4e36-95f8-7cd5a4ba6190"
      );
      resp.data.forEach(element => {
        console.log(resp.data);
        element.active = false;
      });
      this.items[0].child = resp.data;
      // return resp.data;
    },
    async getCustomizationTagCategory() {
      let resp = await getAllCustomizationTagCategoryAsync(
        "33104d76-30be-4e36-95f8-7cd5a4ba6190"
      );
      resp.data.forEach(element => {
        element.active = false;
      });
      this.items[1].child = resp.data;
    },
    clickAction(categoryId, customId) {
      this.$store.dispatch("updateCategoryId", categoryId);
      this.$store.dispatch("updateCustom", customId);
    },
    updateStoreCategory() {
      this.$store.dispatch("updateTagCategory", this.tagCategory);
    },
    async init() {
      await this.getBehaviorTagCategory();
      await this.getCustomizationTagCategory();
    }
  },
  computed: {},
  watch: {},
  mounted() {
    this.init();
    // this.getCustomizationTagCategory().then(res => {

    //   res.forEach(element => {
    //     this.tagCategory.push(element);
    //   });
    //   this.$store.dispatch("updateCustomTagCategory", res);
    // });
    // this.getBehaviorTagCategory().then(res => {
    //   this.items[0].child = res;
    //   res.forEach(element => {
    //     this.tagCategory.push(element);
    //   });
    // });
    this.updateStoreCategory();
  }
};
</script>