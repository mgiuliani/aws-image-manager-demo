<template>
  <div class="card">
      <img :src="model.url" alt="100%x280" style="height: 280px; width: 100%; display: block;" data-holder-rendered="true" />
      <button type="button" class="btn btn-outline-danger btn-sm delete" v-on:click="deleteImage">X</button>
      <p class="card-text">
          <button class="btn btn-sm tag" style="margin:2px;" v-for="tag in model.tags">
            {{ tag.name }} <span class="badge badge-secondary">{{tag.confidence}}</span>
          </button>
      </p>
  </div>
</template>

<script>
import Axios from 'axios';

export default {
  props: ['model'],
  name: 'image-thumbnail',
  methods: {
        deleteImage() {
            Axios.delete(`http://localhost:50277/api/images/` + this.model.key)
                .then(response => {
                    this.$emit('imageDeleted');
                });
        }
    }
}
</script>