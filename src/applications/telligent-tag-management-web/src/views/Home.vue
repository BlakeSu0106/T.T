<template>
  <div>
    <Header />
    <div>
      <Navigation />
      <v-content>
        <v-container>
          <router-view />
        </v-container>
      </v-content>
    </div>
  </div>
</template>
<script>
export default {
  data: function() {
    return {
      items: [
        { title: "行為標籤分類", child: [{}] },
        { title: "自訂標籤分類", child: [] }
      ]
    };
  },
  methods: {},
  computed: {},
  mounted() {},
  watch: {},
  components: {
    Header: () => import("@/components/layout/Header"),
    Navigation: () => import("@/components/layout/Navigation")
  }
};
</script>
