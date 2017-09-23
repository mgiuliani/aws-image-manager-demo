<template>
  <div>
    <section class="jumbotron text-center">
      <div class="container">
        <h1 class="jumbotron-heading" v-if="loading">Loading Images...</h1>
        <h1 class="jumbotron-heading" v-if="!loading">You have {{ imageList.length }} images.</h1>
        <p v-if="!loading" class="lead text-muted">Wanna add some more?</p>
        <image-uploader @uploadSuccess="updatePhotoList"></image-uploader>
      </div>
    </section>

    <div class="album text-muted">
      <div class="container">
        <div class="row">
          <image-thumbnail v-for="image in imageList" @imageDeleted="updatePhotoList" v-bind:model="image"></image-thumbnail>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Axios from 'axios';
import ImageUploader from './ImageUploader.vue';
import ImageThumbnail from './ImageThumbnail.vue';


export default {
  name: 'image-list',
  data () {
    return {
      imageList: [],
      loading: true
    }
  },
  created: function () {
    this.updatePhotoList();
  },
  methods: {
      updatePhotoList() {
          this.loading = true;
          Axios.get(`/api/images`)
              .then(response => {
                  this.loading = false;
                  this.imageList = response.data;
                  console.log(this.imageList);
              })
              .catch(e => {
                  this.loading = false;
                  this.errors.push(e)
              });
      }
  },
  components: {
    ImageThumbnail,
    ImageUploader
  }
}
</script>