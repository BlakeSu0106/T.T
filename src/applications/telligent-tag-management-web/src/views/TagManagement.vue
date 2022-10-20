<template>
  <v-data-table
    :headers="headers"
    :items="tags"
    class="elevation-0"
    hide-default-footer
    @mouseover="selectItem(item)"
    @mouseleave="unselectItem()"
  >
    <template v-slot:top>
      <v-toolbar flat>
        <v-spacer></v-spacer>
        <div class="text-no-wrap pr-5">共有{{ tags.length }}筆資料</div>
        <v-btn color="#00bfaf" dark class="mb-2 mx-2" @click="editCustomTagList = true">編輯自訂人工貼標分類</v-btn>
        <v-btn color="#00bfaf" dark class="mb-2" @click="createNewTag = true">
          <v-icon left>mdi-plus</v-icon>新增標籤
        </v-btn>
        <!-- 編輯人工標籤分類 -->
        <v-dialog v-model="editCustomTagList" max-width="500px">
          <v-card>
            <v-card-title>
              <span class="text-h6">編輯自訂人工貼標分類</span>
            </v-card-title>
            <v-data-table
              :items="customTagCategory"
              :headers="customHeaders"
              class="elevation-0"
              hide-default-footer
            ></v-data-table>
          </v-card>
        </v-dialog>
        <!-- 編輯標籤 Dialog -->
        <v-dialog v-model="editCustomTag" max-width="500px">
          <v-card>
            <v-form v-model="valid">
              <v-card-title>
                <span class="text-h5">編輯標籤</span>
              </v-card-title>
              <v-card-text>
                <v-container>
                  <span>同一標籤分類下，標籤名稱不可重複。限定25個半形字以內</span>
                  <div>
                    <v-text-field label="標籤名稱" :rules="[rules.required]" :value="[selectedName]"></v-text-field>
                  </div>
                </v-container>
              </v-card-text>
              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="blue darken-1" outlined @click="editCustomTag = false">取消</v-btn>
                <v-btn color="primary" depressed @click="editCustomTag = false">確定</v-btn>
              </v-card-actions>
            </v-form>
          </v-card>
        </v-dialog>
        <!-- 新增標籤 Dialog -->
        <v-dialog v-model="createNewTag" max-width="500px">
          <v-card>
            <v-card-title>
              <span class="text-h5">新增標籤</span>
            </v-card-title>
            <v-card-text>
              <v-container>
                <span>同一標籤分類下，標籤名稱不可重複。限定25個半形字以內。</span>
                <br />
                <span>如輸入名稱與停用中標籤相同，則會直接啟用該標籤。</span>
                <br />
                <strong>標籤分類</strong>
                <div>
                  <v-select
                    :items="tagCategorys"
                    item-text="name"
                    item-value="id"
                    :rules="[(v) => !!v || '必須選擇標籤類別']"
                    dense
                    required
                    @change="clearName=''"
                    v-model="selectedCategory"
                  ></v-select>
                  <br />
                  <strong>標籤名稱</strong>

                  <v-text-field
                    v-model="textBox"
                    @keypress.enter="addChips"
                    @keydown.delete="backspaceDelete"
                  >
                    <template v-slot:prepend-inner>
                      <div v-for="(chipText , index) in chipData" :key="index">
                        <v-chip class="ma-1" close @click:close="deleteChip(index)">{{chipText}}</v-chip>
                      </div>
                    </template>
                  </v-text-field>
                </div>
              </v-container>
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="blue darken-1" outlined @click="createNewTag = false">取消</v-btn>
              <v-btn color="primary" depressed @click="createTag()">確定</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
        <!-- 刪除標籤 Dialog -->
        <v-dialog v-model="deletedialog" max-width="400px">
          <v-card>
            <v-card-title class="text-h5">確認刪除</v-card-title>
            <v-card-text>
              <span>是否確定刪除此標籤</span>
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="blue darken-1" outlined text @click="deletedialog = false">取消</v-btn>
              <v-btn color="primary" depressed @click="deletedialog = false,deleteTag()">確定</v-btn>
              <v-spacer></v-spacer>
            </v-card-actions>
          </v-card>
        </v-dialog>
      </v-toolbar>
    </template>
    <template v-slot:body="{ items }">
      <tbody>
        <tr
          v-for="item in items"
          :key="item.id"
          @mouseover="selectItem(item)"
          @mouseleave="unSelectedItem()"
        >
          <td>{{ item.name }}</td>
          <td v-if="customId === '1'">{{ item.eventBinding ? "有":"無" }}</td>
          <td v-else>--</td>
          <td>{{ item.usedQuantity }}</td>
          <td>
            {{ new Date(item.creationTime).toLocaleString() }}
            <br />
            {{ item.creatorName }}
          </td>
          <td>
            <v-toolbar
              v-if="item === selectedItem && customId === '2'"
              rounded="pill"
              dense
              floating
              height="40px"
              elevation="3"
            >
              <v-btn icon @click="editCustomTag = true">
                <v-icon>mdi-pencil</v-icon>
              </v-btn>
              <v-btn icon @click="deletedialog = true, changeSelectTag(item.id)">
                <v-icon>mdi-delete</v-icon>
              </v-btn>
            </v-toolbar>
          </td>
        </tr>
      </tbody>
    </template>
    <!-- <template v-slot:item.actions="{ item }"> -->
    <template v-slot:no-data>
      <v-btn color="primary">Reset</v-btn>
    </template>
  </v-data-table>
</template>

<script>
import { getTagByCategoryAsync } from "@/apis/tagApi";
import { deleteTagAsync } from "@/apis/tagApi";
import { postCreateNewTagAsync } from "@/apis/tagApi";

export default {
  data: function() {
    return {
      editCustomTagList: false,
      valid: false,
      selectedName: "無",
      selectedCategory: "",
      newTagName: "",
      createNewTag: false,
      editedItem: {},
      dialog: false,
      editCustomTag: false,
      deletedialog: false,
      selectedItem: false,
      customId: "1",
      items: [],
      customHeaders: [
        {
          text: "標籤名稱",
          value: "name",
          class: "el-header-grey"
        },
        {
          text: "標籤數量",
          value: "tagQuantity",
          class: "el-header-grey"
        },
        { text: "", value: "action", class: "el-header-grey" }
      ],
      headers: [
        {
          text: "標籤名稱",
          align: "start",
          sortable: false,
          value: "name",
          class: "el-header-grey",
          width: "60%"
        },
        {
          text: "已套用於項目/事件",
          value: "eventBinding",
          class: "el-header-grey"
        },
        { text: "貼標人數", value: "usedQuantity", class: "el-header-grey" },
        { text: "建立時間", value: "creationTime", class: "el-header-grey" },
        { text: "", value: "action", class: "el-header-grey", width: "10%" }
      ],
      tags: [],
      customTagCategory: [],
      tagCategorys: [],
      categoryId: "88232da0-dd7a-11ec-a719-0242ac170002",
      rules: {
        required: value => !!value || "標籤格式錯誤",
        length: len => v => (v || "").length <= len || "標籤字數錯誤"
      },
      selectedId: "",
      newTagData: [
        {
          companyId: "",
          categoryType: 0,
          categoryId: "",
          name: "",
          activationStatus: true
        }
      ],
      textBox: "",
      chipData: []
    };
  },
  methods: {
    async getTagByCategory() {
      let resp = await getTagByCategoryAsync(this.categoryId);
      return resp.data;
    },
    async deleteTag() {
      let resp = await deleteTagAsync(this.selectedId);
      this.$forceUpdate();
      this.$router.go();
    },
    async postNewTag() {
      try {
        let resp = await postCreateNewTagAsync(this.newTagData);
      } catch (error) {
        console.log("error", error);
      }
      console.log(resp);
    },
    selectItem(item) {
      this.selectedItem = item;
      this.selectedName = item.name;
    },
    unSelectedItem(item) {
      this.selectedItem = false;
    },
    changeSelectTag(tagId) {
      this.selectedId = tagId;
    },
    clearName() {
      this.newTagName = "";
    },
    addChips() {
      if (this.textBox.length) {
        this.chipData.push(this.textBox);
        this.textBox = "";
      }
    },
    deleteChip(index) {
      this.chipData.splice(index, 1);
    },
    backspaceDelete() {
      this.chipData.pop();
    },
    createTag() {
      this.createNewTag = false;
      let type = this.selectedCategory.substr(0, 1);
      let tag = {
        companyId: "33104d76-30be-4e36-95f8-7cd5a4ba6190",
        categoryId: this.selectedCategory,
        categoryType: type,
        name: this.newTagName
      };
      this.newTagData.push(tag);

      // this.newTagData[0].companyId = "33104d76-30be-4e36-95f8-7cd5a4ba6190";
      // this.newTagData[0].categoryId = this.selectedCategory;
      // this.newTagData[0].categoryType = type === 8 ? 0 : 1;
      // this.newTagData[0].name = this.newTagName;
      this.postNewTag();
      // this.$forceUpdate();
      // this.$router.go();
    }
  },
  computed: {
    formTitle() {
      return this.editedIndex === 1 ? "New Item" : "編輯標籤";
    }
  },
  watch: {
    "$store.state.categoryId": function() {
      this.categoryId = this.$store.state.categoryId;
      this.getTagByCategory().then(res => {
        this.tags = res;
      });
    },
    "$store.state.customId": function() {
      this.customId = this.$store.state.customId;
    },
    "$store.state.tagCategory": function() {
      this.tagCategorys = this.$store.state.tagCategory;
    },
    "$store.state.customTagCategory": function() {
      this.customTagCategory = this.$store.state.customTagCategory;
    }
  },
  mounted() {
    this.getTagByCategory().then(res => {
      this.tags = res;
    });
  }
};
</script>
<style>
.el-header-grey {
  background-color: #e9ecef;
}
</style>
