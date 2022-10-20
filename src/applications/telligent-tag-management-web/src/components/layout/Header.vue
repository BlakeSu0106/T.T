<template>
     <v-app-bar clipped-left
      app elevation="1"
      color="white"
    >
      <div class="d-flex align-center">
        <h3>標籤管理</h3>
      </div>
      <v-spacer></v-spacer>
            <div class="d-flex align-center mt-8" >
                 <v-text-field
                 color="gray"
              solo  filled 
              label="請輸入關鍵字"
              append-icon="mdi-magnify"
            ></v-text-field>
      </div>
    </v-app-bar> 
</template>

<script>
export default {
  data: function () {
    return {
      items:[
        {title:'行為標籤分類',child:[{}]},{title:'自訂標籤分類',child:[]}
      ]
    };
  },
  methods: {

  },
  computed: {
  },
  watch: {
  }
};
</script>
<style>
.v-text-field .v-input__control .v-input__slot {
    min-height: auto !important;
    display: flex !important;
    align-items: center !important;
  }
</style>
